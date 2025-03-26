using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
    public interface IBookingRepository
    {
        Task<bool> Create(Booking create);
        Task<List<Booking>> GetAll();
        Task<bool> Update(Booking update);
        Task<bool> Delete(int id);
        Task<Booking> GetById(int id);
        Task<List<Booking>> Search(BookingSearchDto search);
    }
}
