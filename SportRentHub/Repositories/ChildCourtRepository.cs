using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;

namespace SportRentHub.Repositories
{
	public class ChildCourtRepository : IChildCourtRepository
	{
		private readonly IDbServices _dbService;

		public ChildCourtRepository(IConfiguration configuration)
		{
			_dbService = new DbServices(configuration);
		}

		public async Task<bool> Create(ChildCourt create)
		{
			var result = await _dbService.EditData(
				"INSERT INTO tbl_child_court (court_id, child_court_name, child_court_description, position, rent_cost) " +
				"VALUES (@CourtId, @ChildCourtName, @ChildCourtDescription, @Position, @RentCost)",
				create);

			return result > 0;
		}

		public async Task<bool> Delete(int id)
		{
			var result = await _dbService.EditData(
				"DELETE FROM tbl_child_court WHERE id = @Id",
				new { Id = id }
			);
			return result > 0;
		}

		public async Task<List<ChildCourt>> GetAll()
		{
			var childCourtList = await _dbService.GetAll<ChildCourt>(
				"SELECT * FROM tbl_child_court ORDER BY id DESC",
				new { }
			);
			return childCourtList;
		}

		public async Task<ChildCourt> GetById(int id)
		{
			var childCourt = await _dbService.GetAsync<ChildCourt>(
				"SELECT * FROM tbl_child_court WHERE id = @Id",
				new { Id = id }
			);
			return childCourt;
		}

		public async Task<List<ChildCourt>> Search(ChildCourtSearchDto search)
		{
			var selectSql = "SELECT * FROM tbl_child_court ";
			var whereSql = " WHERE 1=1 ";

			if (search.IdLst != null && search.IdLst.Any())
			{
				whereSql += " AND id IN @IdLst";
			}
			if (search.Id != null)
			{
				whereSql += " AND id = @Id";
			}
			if (search.CourtId != null)
			{
				whereSql += " AND court_id = @CourtId";
			}
			if (!string.IsNullOrEmpty(search.ChildCourtName))
			{
				whereSql += " AND child_court_name = @ChildCourtName";
			}

			whereSql += " ORDER BY id DESC";

			var childCourtList = await _dbService.GetAll<ChildCourt>(selectSql + whereSql, search);
			return childCourtList;
		}

		public async Task<bool> Update(ChildCourt update)
		{
			var hasChanged = false;
			var updateSql = "UPDATE tbl_child_court SET ";

			if (update.CourtId > 0)
			{
				updateSql += "court_id = @CourtId, ";
				hasChanged = true;
			}
			if (!string.IsNullOrEmpty(update.ChildCourtName))
			{
				updateSql += "child_court_name = @ChildCourtName, ";
				hasChanged = true;
			}
			if (!string.IsNullOrEmpty(update.ChildCourtDescription))
			{
				updateSql += "child_court_description = @ChildCourtDescription, ";
				hasChanged = true;
			}
			if (!string.IsNullOrEmpty(update.Position))
			{
				updateSql += "position = @Position, ";
				hasChanged = true;
			}
			if (update.RentCost > 0)
			{
				updateSql += "rent_cost = @RentCost, ";
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