using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
    public interface ICourtRepository
    {
        Task<bool> Create(Court create);
        Task<List<Court>> GetAll();
        Task<bool> Update(Court court);
        Task<bool> Delete(int id);
        Task<Court> GetById(int id);
        Task<List<Court>> Search(CourtSearchDto search);
    }
}
