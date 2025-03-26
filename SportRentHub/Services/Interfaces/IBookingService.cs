using SportRentHub.Entities.DTOs.Booking;

namespace SportRentHub.Services.Interfaces
{
    public interface IBookingService
    {
        Task<bool> Create(BookingCreateDto create);
        Task<List<BookingDto>> GetAll();
        Task<bool> Delete(int id);
        Task<BookingDto> GetById(int id);
        Task<bool> Update(BookingUpdateDto update);
        Task<List<BookingDto>> Search(BookingSearchDto search);
    }
}
