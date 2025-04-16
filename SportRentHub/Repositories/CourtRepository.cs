using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;

namespace SportRentHub.Repositories
{
    public class CourtRepository : ICourtRepository
    {
        private readonly IDbServices _dbService;

        public CourtRepository(IConfiguration configuration)
        {
            _dbService = new DbServices(configuration);
        }

        public async Task<bool> Create(Court court)
        {
            var result = await _dbService.EditData(
                "INSERT INTO tbl_court (user_id, court_name, court_description, district, ward, street, min_price, max_price, create_date, update_date, contact_person, contact_phone, status, images, start_time, end_time) " +
				"VALUES (@UserId, @CourtName, @CourtDescription, @District, @Ward, @Street, @MinPrice, @MaxPrice, @CreateDate, @UpdateDate, @ContactPerson, @ContactPhone, @Status, @Images, @StartTime, @EndTime)",
                court);

            return result > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _dbService.EditData(
                "DELETE FROM tbl_court WHERE id = @Id",
                new { Id = id }
            );
            return result > 0;
        }

        public async Task<List<Court>> GetAll()
        {
            var courtList = await _dbService.GetAll<Court>(
                "SELECT * FROM tbl_court ORDER BY id DESC",
                new { }
            );
            return courtList;
        }

        public async Task<Court> GetById(int id)
        {
            var court = await _dbService.GetAsync<Court>(
                "SELECT * FROM tbl_court WHERE id = @Id",
                new { Id = id }
            );
            return court;
        }

        public async Task<List<Court>> Search(CourtSearchDto search)
        {
            var selectSql = "SELECT * FROM tbl_court ";
            var whereSql = " WHERE 1=1 ";

            if (search.IdLst != null && search.IdLst.Any())
            {
                whereSql += " AND id IN @IdLst";
            }
            if (search.Id != null)
            {
                whereSql += " AND id = @Id";
            }
            if (!string.IsNullOrEmpty(search.CourtName))
            {
                whereSql += " AND court_name = @CourtName";
            }
            if (!string.IsNullOrEmpty(search.District))
            {
                whereSql += " AND district = @District";
            }
            if (!string.IsNullOrEmpty(search.Ward))
            {
                whereSql += " AND ward = @Ward";
            }
            if (!string.IsNullOrEmpty(search.Street))
            {
                whereSql += " AND street = @Street";
            }
            if (search.Status != null)
            {
                whereSql += " AND status = @Status";
            }

            whereSql += " ORDER BY id DESC";

            var courtList = await _dbService.GetAll<Court>(selectSql + whereSql, search);
            return courtList;
        }

        public async Task<bool> Update(Court court)
        {
            var hasChanged = false;
            var updateSql = "UPDATE tbl_court SET ";

            if (!string.IsNullOrEmpty(court.CourtName))
            {
                updateSql += "court_name = @CourtName, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(court.CourtDescription))
            {
                updateSql += "court_description = @CourtDescription, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(court.District))
            {
                updateSql += "district = @District, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(court.Ward))
            {
                updateSql += "ward = @Ward, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(court.Street))
            {
                updateSql += "street = @Street, ";
                hasChanged = true;
            }
            if (court.MinPrice > 0)
            {
                updateSql += "min_price = @MinPrice, ";
                hasChanged = true;
            }
            if (court.MaxPrice > 0)
            {
                updateSql += "max_price = @MaxPrice, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(court.ContactPerson))
            {
                updateSql += "contact_person = @ContactPerson, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(court.ContactPhone))
            {
                updateSql += "contact_phone = @ContactPhone, ";
                hasChanged = true;
            }
            if (court.Status >= 0)
            {
                updateSql += "status = @Status, ";
                hasChanged = true;
            }
			if (court.Images != null)
			{
				updateSql += "images = @Images, ";
				hasChanged = true;
			}
            if(court.StartTime != TimeSpan.MinValue)
            {
                updateSql += " start_time = @StartTime, ";
                hasChanged = true;
            }
            if (court.EndTime != TimeSpan.MaxValue)
            {
                updateSql += "end_time = @EndTime, ";
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

            var result = await _dbService.EditData(updateSql, court);
            return result > 0;
        }
    }
}
