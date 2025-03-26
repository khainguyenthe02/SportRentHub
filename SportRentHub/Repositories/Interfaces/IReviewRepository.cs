
using SportRentHub.Entities.DTOs.Review;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
    public interface IReviewRepository
    {
        Task<bool> Create(Review create);
        Task<List<Review>> GetAll();
        Task<bool> Update(Review update);
        Task<bool> Delete(int id);
        Task<Review> GetById(int id);
        Task<List<Review>> Search(ReviewSearchDto search);
    }
}
