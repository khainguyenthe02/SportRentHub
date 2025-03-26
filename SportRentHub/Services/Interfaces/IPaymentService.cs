using SportRentHub.Entities.DTOs.Payment;
namespace SportRentHub.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> Create(PaymentCreateDto create);
        Task<List<PaymentDto>> GetAll();
        Task<bool> Delete(int id);
        Task<PaymentDto> GetById(int id);
        Task<bool> Update(PaymentUpdateDto update);
        Task<List<PaymentDto>> Search(PaymentSearchDto search);
    }
}
