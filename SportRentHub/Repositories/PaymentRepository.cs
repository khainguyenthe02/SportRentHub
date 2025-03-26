using SportRentHub.Entities.DTOs.Payment;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SportRentHub.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDbServices _dbService;

        public PaymentRepository(IConfiguration configuration)
        {
            _dbService = new DbServices(configuration);
        }

        public async Task<bool> Create(Payment payment)
        {
            var result = await _dbService.EditData(
                "INSERT INTO tbl_payment (user_id, booking_id, create_date, price, type) " +
                "VALUES (@UserId, @BookingId, @CreateDate, @Price, @Type)",
                payment);

            return result > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _dbService.EditData(
                "DELETE FROM tbl_payment WHERE id = @Id",
                new { Id = id }
            );
            return result > 0;
        }

        public async Task<List<Payment>> GetAll()
        {
            var paymentList = await _dbService.GetAll<Payment>(
                "SELECT * FROM tbl_payment ORDER BY id DESC",
                new { }
            );
            return paymentList;
        }

        public async Task<Payment> GetById(int id)
        {
            var payment = await _dbService.GetAsync<Payment>(
                "SELECT * FROM tbl_payment WHERE id = @Id",
                new { Id = id }
            );
            return payment;
        }

        public async Task<List<Payment>> Search(PaymentSearchDto search)
        {
            var selectSql = "SELECT * FROM tbl_payment ";
            var whereSql = " WHERE 1=1 ";

            if (search.IdLst != null && search.IdLst.Any())
            {
                whereSql += " AND id IN @IdLst";
            }
            if (search.Id.HasValue)
            {
                whereSql += " AND id = @Id";
            }
            if (search.UserId.HasValue)
            {
                whereSql += " AND user_id = @UserId";
            }
            if (search.BookingId.HasValue)
            {
                whereSql += " AND booking_id = @BookingId";
            }
            whereSql += " ORDER BY id DESC";

            var paymentList = await _dbService.GetAll<Payment>(selectSql + whereSql, search);
            return paymentList;
        }

        public async Task<bool> Update(Payment payment)
        {
            var hasChanged = false;
            var updateSql = "UPDATE tbl_payment SET ";

            if (payment.Price > 0)
            {
                updateSql += "price = @Price, ";
                hasChanged = true;
            }
            if (payment.Type > 0)
            {
                updateSql += "type = @Type, ";
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

            var result = await _dbService.EditData(updateSql, payment);
            return result > 0;
        }
    }
}
