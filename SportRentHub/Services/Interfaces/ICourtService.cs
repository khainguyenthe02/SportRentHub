
using SportRentHub.Entities.DTOs.Court;

namespace SportRentHub.Services.Interfaces
{
    public interface ICourtService
    {
        Task<bool> Create(CourtCreateDto create);
        Task<List<CourtDto>> GetAll();
        Task<bool> Delete(int id);
        Task<CourtDto> GetById(int id);
        Task<bool> Update(CourtUpdateDto update);
        Task<List<CourtDto>> Search(CourtSearchDto search);
    }
}
