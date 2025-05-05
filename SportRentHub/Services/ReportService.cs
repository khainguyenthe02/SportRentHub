using Mapster;
using Serilog;
using SportRentHub.Entities.DTOs.Report;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SportRentHub.Entities.Enum;

namespace SportRentHub.Services
{
	public class ReportService : IReportService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ReportService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<bool> Create(ReportCreateDto create)
		{
			try
			{
				var report = create.Adapt<Report>();
				report.CreateDate = DateTime.Now;
				report.Status = (int)ReportStatus.PENDING;

				var result = await _repositoryManager.ReportRepository.Create(report);
				return result;
			}
			catch (Exception ex)
			{
				Log.Error("[ReportService] Error creating report: {@CreateDto}", create, ex.Message);
				throw;
			}
		}

		public async Task<bool> Delete(int id)
		{
			try
			{
				var result = await _repositoryManager.ReportRepository.Delete(id);
				return result;
			}
			catch (Exception ex)
			{
				Log.Error("[ReportService] Error deleting report with ID: {Id}", id, ex.Message);
				throw;
			}
		}

		public async Task<List<ReportDto>> GetAll()
		{
			try
			{
				var reports = await _repositoryManager.ReportRepository.GetAll();
				return await FilterData(reports.Adapt<List<ReportDto>>());
			}
			catch (Exception ex)
			{
				Log.Error("[ReportService] Error fetching all reports.", ex.Message);
				throw;
			}
		}

		public async Task<ReportDto> GetById(int id)
		{
			try
			{
				var report = await _repositoryManager.ReportRepository.GetById(id);
				var filteredList = await FilterData(new List<ReportDto> { report.Adapt<ReportDto>() });

				return filteredList.FirstOrDefault();
			}
			catch (Exception ex)
			{
				Log.Error("[ReportService] Error fetching report with ID: {Id}", id, ex.Message);
				throw;
			}
		}

		public async Task<List<ReportDto>> Search(ReportSearchDto search)
		{
			try
			{
				var reports = await _repositoryManager.ReportRepository.Search(search);
				return await FilterData(reports.Adapt<List<ReportDto>>());
			}
			catch (Exception ex)
			{
				Log.Error("[ReportService] Error searching reports: {@Search}", search, ex.Message);
				throw;
			}
		}

		public async Task<bool> Update(ReportUpdateDto update)
		{
			try
			{
				var existingReport = await _repositoryManager.ReportRepository.GetById(update.Id);
				if (existingReport == null)
				{
					Log.Warning("[ReportService] Report not found with ID: {Id}", update.Id);
					return false;
				}
				update.Adapt(existingReport);

				var result = await _repositoryManager.ReportRepository.Update(existingReport);
				if (result)
				{
					var reportSearch = new ReportSearchDto
					{
						CourtId = update.CourtId,
						Status = (int)ReportStatus.ACCEPTED
					};
					var reportAcceptedList = await _repositoryManager.ReportRepository.Search(reportSearch);
					if (reportAcceptedList.Count > 10)
					{
						var courtUpdate = new Court
						{
							Id = existingReport.CourtId,
							Status = (int)CourtStatus.INACTIVE
						};
						var deleteCourt = await _repositoryManager.CourtRepository.Update( courtUpdate);
						if (deleteCourt)
						{
							var userReports = await _repositoryManager.ReportRepository.Search(new ReportSearchDto
							{
								CourtId = existingReport.UserId,
								Status = (int)ReportStatus.ACCEPTED
							});
							var distinctCourtIds = userReports.Select(r => r.CourtId).Distinct().ToList();
							if (distinctCourtIds.Count > 10)
							{
								var userUpdate = new User
								{
									Id = existingReport.UserId,
									Status = (int)AccountStatus.BLOCK
								};
								var deleteUser = await _repositoryManager.UserRepository.Update(userUpdate);
								return deleteUser;
							}
						}
					}
					return true;
				}
				else
				{
					Log.Warning("[ReportService] Failed to update report with ID: {Id}", update.Id);
					return false;
				}
			}
			catch (Exception ex)
			{
				Log.Error("[ReportService] Error updating report: {@Update}", update, ex.Message);
				throw;
			}
		}

		public async Task<List<ReportDto>> FilterData(List<ReportDto> lst)
		{
			if (lst.Any())
			{
				// Adapter user
				var userIdLst = lst.Where(item => item.UserId != 0).Select(item => item.UserId).ToList();
				if (userIdLst.Any())
				{
					var searchUser = new UserSearchDto
					{
						IdLst = userIdLst,
					};
					var userLst = (await _repositoryManager.UserRepository.Search(searchUser))
						.ToDictionary(u => u.Id, u => (u.Fullname, u.PhoneNumber));
					if (userLst.Any())
					{
						foreach (var item in lst)
						{
							if (item.UserId != 0 && userLst.ContainsKey(item.UserId))
							{
								item.UserFullname = userLst[item.UserId].Fullname;
							}
						}
					}
				}

				// Adapter court
				var courtIdLst = lst.Where(item => item.CourtId != 0).Select(item => item.CourtId).ToList();
				if (courtIdLst.Any())
				{
					var searchCourt = new CourtSearchDto
					{
						IdLst = courtIdLst,
					};
					var courtLst = (await _repositoryManager.CourtRepository.Search(searchCourt))
						.ToDictionary(c => c.Id, c => new Court
						{
							Id = c.Id,
							CourtName = c.CourtName,
							Ward = c.Ward,
							Street = c.Street,
							District = c.District
						});

					if (courtLst.Any())
					{
						foreach (var item in lst)
						{
							if (item.CourtId != 0 && courtLst.ContainsKey(item.CourtId))
							{
								var court = courtLst[item.CourtId];
								item.CourtName = court.CourtName;
								item.CourtWard = court.Ward;
								item.CourtStreet = court.Street;
								item.CourtDistrict = court.District;
							}
						}
					}
				}
			}
			return lst;
		}
	}
}