using Mapster;
using Serilog;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.DTOs.Review;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IRepositoryManager _repositoryManager;

        public ReviewService(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
        }

        public async Task<bool> Create(ReviewCreateDto create)
        {
            try
            {
                var review = create.Adapt<Review>();
                review.CreateDate = DateTime.Now;
                review.UpdateDate = DateTime.Now;

                var result = await _repositoryManager.ReviewRepository.Create(review);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[ReviewService] Error creating review: {@createDto}", create, ex.Message);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var result = await _repositoryManager.ReviewRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[ReviewService] Error deleting review with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<ReviewDto>> GetAll()
        {
            try
            {
                var reviews = await _repositoryManager.ReviewRepository.GetAll();
                return await FilterData( reviews.Adapt<List<ReviewDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[ReviewService] Error fetching all reviews.", ex.Message);
                throw;
            }
        }

        public async Task<ReviewDto> GetById(int id)
        {
            try
            {
                var review = await _repositoryManager.ReviewRepository.GetById(id);
                return (await FilterData( new List<ReviewDto> { review?.Adapt<ReviewDto>() })).First();
            }
            catch (Exception ex)
            {
                Log.Error("[ReviewService] Error fetching review with ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<ReviewDto>> Search(ReviewSearchDto search)
        {
            try
            {
                var reviews = await _repositoryManager.ReviewRepository.Search(search);
                return await FilterData( reviews.Adapt<List<ReviewDto>>());
            }
            catch (Exception ex)
            {
                Log.Error("[ReviewService] Error searching reviews: {@Search}", search, ex.Message);
                throw;
            }
        }

        public async Task<bool> Update(ReviewUpdateDto update)
        {
            try
            {
                var existingReview = await _repositoryManager.ReviewRepository.GetById(update.Id);
                if (existingReview == null)
                {
                    Log.Warning("[ReviewService] Review not found with ID: {Id}", update.Id);
                    return false;
                }
                update.Adapt(existingReview);
                existingReview.UpdateDate = DateTime.Now;

                var result = await _repositoryManager.ReviewRepository.Update(existingReview);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[ReviewService] Error updating review: {@Update}", update, ex.Message);
                throw;
            }
        }
        public async Task<List<ReviewDto>> FilterData(List<ReviewDto> lst)
        {
            if (lst.Any())
            {
                //adapter user
                var userIdLst = lst.Where(item => item.UserId != 0).Select(item => item.UserId).ToList();
                if (userIdLst.Any())
                {
                    var searchUser = new UserSearchDto
                    {
                        IdLst = userIdLst,
                    };
                    var userLst = (await _repositoryManager.UserRepository.Search(searchUser))
                        .ToDictionary(u => u.Id, u => (u.Fullname, u.Username));
                    if (userLst.Any())
                    {
                        foreach (var item in lst)
                        {
                            if (item.UserId != 0 && userLst.ContainsKey(item.UserId))
                            {
                                item.UserFullname = userLst[item.UserId].Fullname;
                                item.Username = userLst[item.UserId].Username;
                            }
                        }
                    }
                }
            }
            return lst;
        }
    }
}
