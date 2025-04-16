using Mapster;
using Serilog;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
	public class ChildCourtService : IChildCourtService
	{
		private readonly IRepositoryManager _repositoryManager;

		public ChildCourtService(IRepositoryManager repositoryManager)
		{
			_repositoryManager = repositoryManager;
		}

		public async Task<bool> Create(ChildCourtCreateDto create)
		{
			try
			{
				var childCourt = create.Adapt<ChildCourt>();

				var result = await _repositoryManager.ChildCourtRepository.Create(childCourt);
				return result;
			}
			catch (Exception ex)
			{
				Log.Error("[ChildCourtService] Error creating child court: {@CreateDto}", create, ex.Message);
				throw;
			}
		}

		public async Task<bool> Delete(int id)
		{
			try
			{
				var result = await _repositoryManager.ChildCourtRepository.Delete(id);
				return result;
			}
			catch (Exception ex)
			{
				Log.Error("[ChildCourtService] Error deleting child court with ID: {Id}", id, ex.Message);
				throw;
			}
		}

		public async Task<List<ChildCourtDto>> GetAll()
		{
			try
			{
				var childCourts = await _repositoryManager.ChildCourtRepository.GetAll();
				return childCourts.Adapt<List<ChildCourtDto>>();
			}
			catch (Exception ex)
			{
				Log.Error("[ChildCourtService] Error fetching all child courts.", ex.Message);
				throw;
			}
		}

		public async Task<ChildCourtDto> GetById(int id)
		{
			try
			{
				var childCourt = await _repositoryManager.ChildCourtRepository.GetById(id);
				if (childCourt == null)
				{
					Log.Warning("[ChildCourtService] Child court not found with ID: {Id}", id);
					return null;
				}
				return childCourt.Adapt<ChildCourtDto>();
			}
			catch (Exception ex)
			{
				Log.Error("[ChildCourtService] Error fetching child court with ID: {Id}", id, ex.Message);
				throw;
			}
		}

		public async Task<List<ChildCourtDto>> Search(ChildCourtSearchDto search)
		{
			try
			{
				var childCourts = await _repositoryManager.ChildCourtRepository.Search(search);
				return childCourts.Adapt<List<ChildCourtDto>>();
			}
			catch (Exception ex)
			{
				Log.Error("[ChildCourtService] Error searching child courts: {@Search}", search, ex.Message);
				throw;
			}
		}

		public async Task<bool> Update(ChildCourtUpdateDto update)
		{
			try
			{
				var existingChildCourt = await _repositoryManager.ChildCourtRepository.GetById(update.Id);
				if (existingChildCourt == null)
				{
					Log.Warning("[ChildCourtService] Child court not found with ID: {Id}", update.Id);
					return false;
				}
				update.Adapt(existingChildCourt);

				var result = await _repositoryManager.ChildCourtRepository.Update(existingChildCourt);
				return result;
			}
			catch (Exception ex)
			{
				Log.Error("[ChildCourtService] Error updating child court: {@Update}", update, ex.Message);
				throw;
			}
		}
	}
}