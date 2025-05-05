using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;

namespace SportRentHub.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly IDbServices _dbService;

        public BookingRepository(IConfiguration configuration)
        {
            _dbService = new DbServices(configuration);
        }

        public async Task<bool> Create(Booking booking)
        {
            var result = await _dbService.EditData(
                "INSERT INTO tbl_booking (user_id, child_court_id, create_date, start_time, end_time, status, price, type, quantity) " +
                "VALUES (@UserId, @ChildCourtId, @CreateDate, @StartTime, @EndTime, @Status, @Price, @Type, @Quantity)",
                booking);

            return result > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _dbService.EditData(
                "DELETE FROM tbl_booking WHERE id = @Id",
                new { Id = id }
            );
            return result > 0;
        }

        public async Task<List<Booking>> GetAll()
        {
            var bookingList = await _dbService.GetAll<Booking>(
                "SELECT * FROM tbl_booking ORDER BY id DESC",
                new { }
            );
            return bookingList;
        }

        public async Task<Booking> GetById(int id)
        {
            var booking = await _dbService.GetAsync<Booking>(
                "SELECT * FROM tbl_booking WHERE id = @Id",
                new { Id = id }
            );
            return booking;
        }

        public async Task<List<Booking>> Search(BookingSearchDto search)
        {
            var selectSql = "SELECT * FROM tbl_booking ";
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
            if (search.ChildCourtId != null)
            {
                whereSql += " AND child_court_id = @ChildCourtId";
            }
            if(search.EndTime != null)
            {
                whereSql += " AND end_time < @EndTime";
            }
            if (search.Status != null)
            {
                whereSql += " AND status = @Status";
            }
            if(search.Type != null)
			{
				whereSql += " AND type = @Type";
			}

			whereSql += " ORDER BY id DESC";

            var bookingList = await _dbService.GetAll<Booking>(selectSql + whereSql, search);
            return bookingList;
        }

        public async Task<bool> Update(Booking booking)
        {
            var hasChanged = false;
            var updateSql = "UPDATE tbl_booking SET ";

            if (booking.UserId > 0)
            {
                updateSql += "user_id = @UserId, ";
                hasChanged = true;
            }
            if (booking.ChildCourtId > 0)
            {
                updateSql += "child_court_id = @ChildCourtId, ";
                hasChanged = true;
            }
            if (booking.StartTime != DateTime.MinValue)
            {
                updateSql += "start_time = @StartTime, ";
                hasChanged = true;
            }
            if (booking.EndTime != DateTime.MinValue)
            {
                updateSql += "end_time = @EndTime, ";
                hasChanged = true;
            }
            if (booking.Status >= 0)
            {
                updateSql += "status = @Status, ";
                hasChanged = true;
            }
            if (booking.Price > 0)
            {
                updateSql += "price = @Price, ";
                hasChanged = true;
            }
			if (booking.Type.HasValue)
			{
				updateSql += "type = @Type, ";
				hasChanged = true;
			}
			if (booking.Quantity.HasValue)
			{
				updateSql += "quantity = @Quantity, ";
				hasChanged = true;
			}
			// Remove the last comma and space if there were any changes

			if (!hasChanged)
            {
                return false;
            }

            if (updateSql.EndsWith(", "))
            {
                updateSql = updateSql.Remove(updateSql.Length - 2);
            }

            updateSql += " WHERE id = @Id";

            var result = await _dbService.EditData(updateSql, booking);
            return result > 0;
        }
    }
}
