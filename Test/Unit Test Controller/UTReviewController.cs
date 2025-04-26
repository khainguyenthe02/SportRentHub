using Microsoft.AspNetCore.Mvc;
using Moq;
using SportRentHub.Controllers;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Review;
using SportRentHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class UTReviewController
    {
        private readonly Mock<IServiceManager> _mockServiceManager;
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly ReviewController _controller;

        public UTReviewController()
        {
            _mockServiceManager = new Mock<IServiceManager>();
            _mockReviewService = new Mock<IReviewService>();

            // Setup service manager to return review service
            _mockServiceManager.Setup(s => s.ReviewService).Returns(_mockReviewService.Object);

            _controller = new ReviewController(_mockServiceManager.Object);
        }

        #region GetReviewById Tests

        // Test 1: Đơn giản - Trả về Ok khi tìm thấy đánh giá theo ID
        [Fact]
        public async Task GetReviewById_ReturnsOk_WhenReviewExists()
        {
            // Arrange
            int reviewId = 1;
            var reviewDto = new ReviewDto { Id = reviewId };
            _mockReviewService.Setup(s => s.GetById(reviewId)).ReturnsAsync(reviewDto);

            // Act
            var result = await _controller.GetReviewById(reviewId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ReviewDto>(okResult.Value);
            Assert.Equal(reviewId, returnValue.Id);
        }

        // Test 2: Đơn giản - Trả về NoContent khi không tìm thấy đánh giá
        [Fact]
        public async Task GetReviewById_ReturnsNoContent_WhenReviewNotFound()
        {
            // Arrange
            int reviewId = 999;
            _mockReviewService.Setup(s => s.GetById(reviewId)).ReturnsAsync((ReviewDto)null);

            // Act
            var result = await _controller.GetReviewById(reviewId);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetReviews Tests

        // Test 3: Đơn giản - Trả về danh sách đánh giá khi có dữ liệu
        [Fact]
        public async Task GetReviews_ReturnsOkWithList_WhenReviewsExist()
        {
            // Arrange
            var reviews = new List<ReviewDto>
            {
                new ReviewDto { Id = 1 },
                new ReviewDto { Id = 2 }
            };
            _mockReviewService.Setup(s => s.GetAll()).ReturnsAsync(reviews);

            // Act
            var result = await _controller.GetReviews();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        // Test 4: Đơn giản - Trả về danh sách rỗng khi không có dữ liệu
        [Fact]
        public async Task GetReviews_ReturnsEmptyList_WhenNoReviewsExist()
        {
            // Arrange
            _mockReviewService.Setup(s => s.GetAll()).ReturnsAsync((List<ReviewDto>)null);

            // Act
            var result = await _controller.GetReviews();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        #endregion

        #region CreateReview Tests

        // Test 5: Đơn giản - Trả về BadRequest khi dữ liệu tạo đánh giá là null
        [Fact]
        public async Task CreateReview_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.CreateReview(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin đánh giá không hợp lệ.", badRequestResult.Value);
        }

        // Test 6: Trung bình - Trả về BadRequest khi tạo đánh giá thất bại
        [Fact]
        public async Task CreateReview_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = new ReviewCreateDto();
            _mockReviewService.Setup(s => s.Create(createDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        // Test 7: Trung bình - Trả về BadRequest khi không lấy được danh sách sau khi tạo
        [Fact]
        public async Task CreateReview_ReturnsBadRequest_WhenGetAllAfterCreateReturnsEmpty()
        {
            // Arrange
            var createDto = new ReviewCreateDto();
            _mockReviewService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockReviewService.Setup(s => s.GetAll()).ReturnsAsync(new List<ReviewDto>());

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        // Test 8: Nâng cao - Trả về Ok với đánh giá đầu tiên khi tạo thành công
        [Fact]
        public async Task CreateReview_ReturnsOkWithFirstReview_WhenCreationSucceeds()
        {
            // Arrange
            var createDto = new ReviewCreateDto();
            var reviews = new List<ReviewDto>
    {
        new ReviewDto {
            Id = 1,
            UserId = 101,
            CourtId = 201,
            Content = "Sân tennis rất tốt, dịch vụ chuyên nghiệp",
            RatingStar = 5,
            CreateDate = DateTime.Now,
            UpdateDate = DateTime.Now,
            Username = "tennisplayer1",
            UserFullname = "Nguyễn Văn A"
        }
    };

            _mockReviewService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockReviewService.Setup(s => s.GetAll()).ReturnsAsync(reviews);

            // Act
            var result = await _controller.CreateReview(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<ReviewDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal(101, returnValue.UserId);
            Assert.Equal(201, returnValue.CourtId);
            Assert.Equal("Sân tennis rất tốt, dịch vụ chuyên nghiệp", returnValue.Content);
            Assert.Equal(5, returnValue.RatingStar);
            Assert.Equal("tennisplayer1", returnValue.Username);
            Assert.Equal("Nguyễn Văn A", returnValue.UserFullname);
        }

        #endregion

        #region UpdateReview Tests

        // Test 9: Đơn giản - Trả về BadRequest khi dữ liệu cập nhật là null
        [Fact]
        public async Task UpdateReview_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.UpdateReview(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin cập nhật không hợp lệ.", badRequestResult.Value);
        }

        // Test 10: Trung bình - Trả về BadRequest khi đánh giá không tồn tại
        [Fact]
        public async Task UpdateReview_ReturnsBadRequest_WhenReviewDoesNotExist()
        {
            // Arrange
            var updateDto = new ReviewUpdateDto { Id = 999 };
            _mockReviewService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync((ReviewDto)null);

            // Act
            var result = await _controller.UpdateReview(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Đánh giá không tồn tại.", badRequestResult.Value);
        }

        // Test 11: Trung bình - Trả về BadRequest khi cập nhật thất bại
        [Fact]
        public async Task UpdateReview_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var updateDto = new ReviewUpdateDto { Id = 1 };
            var existingReview = new ReviewDto { Id = 1 };

            _mockReviewService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(existingReview);
            _mockReviewService.Setup(s => s.Update(updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateReview(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật đánh giá thất bại.", badRequestResult.Value);
        }

        // Test 12: Nâng cao - Trả về Ok khi cập nhật thành công
        [Fact]
        public async Task UpdateReview_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var updateDto = new ReviewUpdateDto { Id = 1 };
            var existingReview = new ReviewDto { Id = 1 };

            _mockReviewService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(existingReview);
            _mockReviewService.Setup(s => s.Update(updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateReview(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        #endregion

        #region DeleteReview Tests

        // Test 13: Đơn giản - Trả về BadRequest khi đánh giá không tồn tại
        [Fact]
        public async Task DeleteReview_ReturnsBadRequest_WhenReviewDoesNotExist()
        {
            // Arrange
            int reviewId = 999;
            _mockReviewService.Setup(s => s.GetById(reviewId)).ReturnsAsync((ReviewDto)null);

            // Act
            var result = await _controller.DeleteReview(reviewId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Đánh giá không tồn tại.", badRequestResult.Value);
        }

        // Test 14: Nâng cao - Trả về Ok khi xóa thành công
        [Fact]
        public async Task DeleteReview_ReturnsOk_WhenDeleteSucceeds()
        {
            // Arrange
            int reviewId = 1;
            var existingReview = new ReviewDto { Id = reviewId };

            _mockReviewService.Setup(s => s.GetById(reviewId)).ReturnsAsync(existingReview);
            _mockReviewService.Setup(s => s.Delete(reviewId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteReview(reviewId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // Test 15: Nâng cao - Xác nhận Delete được gọi với đúng ID
        [Fact]
        public async Task DeleteReview_CallsDeleteWithCorrectId_WhenReviewExists()
        {
            // Arrange
            int reviewId = 1;
            var existingReview = new ReviewDto { Id = reviewId };

            _mockReviewService.Setup(s => s.GetById(reviewId)).ReturnsAsync(existingReview);
            _mockReviewService.Setup(s => s.Delete(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            await _controller.DeleteReview(reviewId);

            // Assert
            _mockReviewService.Verify(s => s.Delete(reviewId), Times.Once);
        }

        #endregion

        #region Search Tests

        // Test 16: Đơn giản - Trả về danh sách rỗng khi không tìm thấy kết quả
        [Fact]
        public async Task Search_ReturnsEmptyList_WhenNoResultsFound()
        {
            // Arrange
            var searchDto = new ReviewSearchDto();
            var emptyList = new List<ReviewDto>();

            _mockReviewService.Setup(s => s.Search(searchDto)).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        // Test 17: Trung bình - Trả về danh sách kết quả khi tìm thấy
        [Fact]
        public async Task Search_ReturnsResults_WhenMatchesFound()
        {
            // Arrange
            var searchDto = new ReviewSearchDto();
            var searchResults = new List<ReviewDto>
            {
                new ReviewDto { Id = 1 },
                new ReviewDto { Id = 2 }
            };

            _mockReviewService.Setup(s => s.Search(searchDto)).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<ReviewDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        #endregion

        #region Advanced Tests

        // Test 18: Nâng cao - Kiểm tra gọi đúng phương thức dịch vụ khi tạo đánh giá
        [Fact]
        public async Task CreateReview_CallsServiceWithCorrectDto()
        {
            // Arrange
            var createDto = new ReviewCreateDto
            {
                UserId = 101,
                CourtId = 201,
                Content = "Sân tennis rất tốt, dịch vụ chuyên nghiệp",
                RatingStar = 5
            };

            _mockReviewService.Setup(s => s.Create(It.IsAny<ReviewCreateDto>())).ReturnsAsync(true);
            _mockReviewService.Setup(s => s.GetAll()).ReturnsAsync(new List<ReviewDto> { new ReviewDto() });

            // Act
            await _controller.CreateReview(createDto);

            // Assert
            _mockReviewService.Verify(s => s.Create(It.Is<ReviewCreateDto>(
                dto => dto.UserId == 101 &&
                       dto.CourtId == 201 &&
                       dto.Content == "Sân tennis rất tốt, dịch vụ chuyên nghiệp" &&
                       dto.RatingStar == 5)),
                Times.Once);
        }

        // Test 19: Nâng cao - Kiểm tra xử lý ngoại lệ khi dịch vụ gây ra ngoại lệ
        [Fact]
        public async Task GetReviewById_HandlesException_WhenServiceThrows()
        {
            // Arrange
            int reviewId = 1;
            _mockReviewService.Setup(s => s.GetById(reviewId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetReviewById(reviewId));
        }

        // Test 20: Nâng cao - Kiểm tra thứ tự gọi các phương thức khi cập nhật
        [Fact]
        public async Task UpdateReview_CallsMethodsInCorrectOrder()
        {
            // Arrange
            var updateDto = new ReviewUpdateDto { Id = 1 };
            var existingReview = new ReviewDto { Id = 1 };
            var sequence = new MockSequence();

            _mockReviewService.InSequence(sequence)
                .Setup(s => s.GetById(updateDto.Id))
                .ReturnsAsync(existingReview);

            _mockReviewService.InSequence(sequence)
                .Setup(s => s.Update(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateReview(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockReviewService.Verify(s => s.GetById(updateDto.Id), Times.Once);
            _mockReviewService.Verify(s => s.Update(updateDto), Times.Once);
        }

        #endregion
    }
}
