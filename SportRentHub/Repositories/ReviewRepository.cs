using SportRentHub.Entities.DTOs.Review;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;

namespace SportRentHub.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly IDbServices _dbService;

        public ReviewRepository(IConfiguration configuration)
        {
            _dbService = new DbServices(configuration);
        }

        public async Task<bool> Create(Review review)
        {
            var result = await _dbService.EditData(
                "INSERT INTO tbl_review (user_id, court_id, content, rating_star, create_date, update_date) " +
                "VALUES (@UserId, @CourtId, @Content, @RatingStar, @CreateDate, @UpdateDate)",
                review);

            return result > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _dbService.EditData(
                "DELETE FROM tbl_review WHERE id = @Id",
                new { Id = id }
            );
            return result > 0;
        }

        public async Task<List<Review>> GetAll()
        {
            var reviewList = await _dbService.GetAll<Review>(
                "SELECT * FROM tbl_review ORDER BY id DESC",
                new { }
            );
            return reviewList;
        }

        public async Task<Review> GetById(int id)
        {
            var review = await _dbService.GetAsync<Review>(
                "SELECT * FROM tbl_review WHERE id = @Id",
                new { Id = id }
            );
            return review;
        }

        public async Task<List<Review>> Search(ReviewSearchDto search)
        {
            var selectSql = "SELECT * FROM tbl_review ";
            var whereSql = " WHERE 1=1 ";

            if (search.IdLst != null && search.IdLst.Any())
            {
                whereSql += " AND id IN @IdLst";
            }
            if (search.Id != null)
            {
                whereSql += " AND id = @Id";
            }
            if (search.UserId != null)
            {
                whereSql += " AND user_id = @UserId";
            }
            if (search.CourtId != null)
            {
                whereSql += " AND court_id = @CourtId";
            }
            whereSql += " ORDER BY id DESC";

            var reviewList = await _dbService.GetAll<Review>(selectSql + whereSql, search);
            return reviewList;
        }

        public async Task<bool> Update(Review review)
        {
            var hasChanged = false;
            var updateSql = "UPDATE tbl_review SET ";

            if (!string.IsNullOrEmpty(review.Content))
            {
                updateSql += "content = @Content, ";
                hasChanged = true;
            }
            if (review.RatingStar > 0)
            {
                updateSql += "rating_star = @RatingStar, ";
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

            var result = await _dbService.EditData(updateSql, review);
            return result > 0;
        }
    }
}
