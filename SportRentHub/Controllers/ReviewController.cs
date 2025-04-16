using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.Review;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public ReviewController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet("id/{id}")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetReviewById(int id)
        {
            var reviewDto = await _serviceManager.ReviewService.GetById(id);
            if (reviewDto == null)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            return Ok(reviewDto);
        }

        [HttpGet("get-list")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> GetReviews()
        {
            var reviews = await _serviceManager.ReviewService.GetAll();
            return Ok(reviews ?? new List<ReviewDto>());
        }

        [HttpPost("create")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto createReviewDto)
        {
            if (createReviewDto == null)
            {
                return BadRequest("Thông tin đánh giá không hợp lệ.");
            }
            var result = await _serviceManager.ReviewService.Create(createReviewDto);
            if (!result) return BadRequest(MessageError.ErrorCreate);

            var lst = new List<ReviewDto>();
            lst = await _serviceManager.ReviewService.GetAll();
            if (!lst.Any()) return BadRequest(MessageError.ErrorCreate);
            return Ok(lst[0]);
        }

        [HttpPut("update")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> UpdateReview([FromBody] ReviewUpdateDto updateReviewDto)
        {
            if (updateReviewDto == null)
            {
                return BadRequest("Thông tin cập nhật không hợp lệ.");
            }
            var review = await _serviceManager.ReviewService.GetById(updateReviewDto.Id);
            if (review == null) return BadRequest("Đánh giá không tồn tại.");

            var result = await _serviceManager.ReviewService.Update(updateReviewDto);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Cập nhật đánh giá thất bại.");
        }

        [HttpDelete("delete")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _serviceManager.ReviewService.GetById(id);
            if (review == null)
            {
                return BadRequest("Đánh giá không tồn tại.");
            }
            await _serviceManager.ReviewService.Delete(id);
            return Ok();
        }
		[HttpPost("search")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> Search(ReviewSearchDto search)
		{
			var searchList = await _serviceManager.ReviewService.Search(search);
			if (!searchList.Any()) return Ok(searchList);
			return Ok(searchList);
		}
	}
}
