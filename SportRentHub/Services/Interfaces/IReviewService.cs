
using SportRentHub.Entities.DTOs.Review;

namespace SportRentHub.Services.Interfaces
{
    public interface IReviewService
    {
        Task<bool> Create(ReviewCreateDto create);
        Task<List<ReviewDto>> GetAll();
        Task<bool> Delete(int id);
        Task<ReviewDto> GetById(int id);
        Task<bool> Update(ReviewUpdateDto update);
        Task<List<ReviewDto>> Search(ReviewSearchDto search);
    }
}
