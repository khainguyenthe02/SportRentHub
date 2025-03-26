using SportRentHub.Entities.DTOs.Payment;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<bool> Create(Payment create);
        Task<List<Payment>> GetAll();
        Task<bool> Update(Payment update);
        Task<bool> Delete(int id);
        Task<Payment> GetById(int id);
        Task<List<Payment>> Search(PaymentSearchDto search);
    }
}
