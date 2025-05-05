using SportRentHub.Entities.DTOs.Report;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SportRentHub.Repositories
{
	public class ReportRepository : IReportRepository
	{
		private readonly IDbServices _dbService;

		public ReportRepository(IConfiguration configuration)
		{
			_dbService = new DbServices(configuration);
		}

		public async Task<bool> Create(Report create)
		{
			var result = await _dbService.EditData(
				"INSERT INTO tbl_report (user_id, court_id, content, create_date, status) " +
				"VALUES (@UserId, @CourtId, @Content, @CreateDate, @Status)",
				create);

			return result > 0;
		}

		public async Task<bool> Delete(int id)
		{
			var result = await _dbService.EditData(
				"DELETE FROM tbl_report WHERE id = @Id",
				new { Id = id }
			);
			return result > 0;
		}

		public async Task<List<Report>> GetAll()
		{
			var reportList = await _dbService.GetAll<Report>(
				"SELECT * FROM tbl_report ORDER BY id DESC",
				new { }
			);
			return reportList;
		}

		public async Task<Report> GetById(int id)
		{
			var report = await _dbService.GetAsync<Report>(
				"SELECT * FROM tbl_report WHERE id = @Id",
				new { Id = id }
			);
			return report;
		}

		public async Task<List<Report>> Search(ReportSearchDto search)
		{
			var selectSql = "SELECT * FROM tbl_report ";
			var whereSql = " WHERE 1=1 ";

			if (search.IdLst != null && search.IdLst.Any())
			{
				whereSql += " AND id IN @IdLst";
			}
			if (search.Id != null)
			{
				whereSql += " AND id = @Id";
			}
			if (search.UserId != null)
			{
				whereSql += " AND user_id = @UserId";
			}
			if (search.CourtId != null)
			{
				whereSql += " AND court_id = @CourtId";
			}
			if (search.Status != null)
			{
				whereSql += " AND status = @Status";
			}

			whereSql += " ORDER BY id DESC";

			var reportList = await _dbService.GetAll<Report>(selectSql + whereSql, search);
			return reportList;
		}

		public async Task<bool> Update(Report update)
		{
			var hasChanged = false;
			var updateSql = "UPDATE tbl_report SET ";

			if (update.UserId > 0)
			{
				updateSql += "user_id = @UserId, ";
				hasChanged = true;
			}
			if (update.CourtId > 0)
			{
				updateSql += "court_id = @CourtId, ";
				hasChanged = true;
			}
			if (!string.IsNullOrEmpty(update.Content))
			{
				updateSql += "content = @Content, ";
				hasChanged = true;
			}
			if (update.CreateDate != DateTime.MinValue)
			{
				updateSql += "create_date = @CreateDate, ";
				hasChanged = true;
			}
			if (update.Status >= 0)
			{
				updateSql += "status = @Status, ";
				hasChanged = true;
			}

			if (!hasChanged)
			{
				return false;
			}

			if (updateSql.EndsWith(", "))
			{
				updateSql = updateSql.Remove(updateSql.Length - 2);
			}

			updateSql += " WHERE id = @Id";

			var result = await _dbService.EditData(updateSql, update);
			return result > 0;
		}
	}
}