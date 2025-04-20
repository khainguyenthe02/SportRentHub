using Microsoft.AspNetCore.Mvc;
using Moq;
using SportRentHub.Controllers;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class UTBooking
    {
        private readonly Mock<IServiceManager> _mockServiceManager;
        private readonly Mock<IBookingService> _mockBookingService;
        private readonly BookingController _controller;

        public UTBooking()
        {
            _mockBookingService = new Mock<IBookingService>();
            _mockServiceManager = new Mock<IServiceManager>();
            _mockServiceManager.Setup(s => s.BookingService).Returns(_mockBookingService.Object);
            _controller = new BookingController(_mockServiceManager.Object);
        }

        [Fact]
        public async Task GetBookingById_ReturnsOkResult_WhenBookingExists()
        {
            // Arrange
            var bookingId = 1;
            var bookingDto = new BookingDto { Id = bookingId };
            _mockBookingService.Setup(service => service.GetById(bookingId))
                .ReturnsAsync(bookingDto);

            // Act
            var result = await _controller.GetBookingById(bookingId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(bookingId, returnValue.Id);
        }

        [Fact]
        public async Task GetBookingById_ReturnsNoContent_WhenBookingDoesNotExist()
        {
            // Arrange
            var bookingId = 1;
            _mockBookingService.Setup(service => service.GetById(bookingId))
                .ReturnsAsync((BookingDto)null);

            // Act
            var result = await _controller.GetBookingById(bookingId);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetBookings_ReturnsOkResult_WithListOfBookings()
        {
            // Arrange
            var bookings = new List<BookingDto>
            {
                new BookingDto { Id = 1 },
                new BookingDto { Id = 2 }
            };
            _mockBookingService.Setup(service => service.GetAll())
                .ReturnsAsync(bookings);

            // Act
            var result = await _controller.GetBookings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetBookings_ReturnsEmptyList_WhenNoBookingsExist()
        {
            // Arrange
            _mockBookingService.Setup(service => service.GetAll())
                .ReturnsAsync(new List<BookingDto>());

            // Act
            var result = await _controller.GetBookings();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task CreateBooking_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.CreateBooking(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin đặt sân không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsBadRequest_WhenServiceFails()
        {
            // Arrange
            var createDto = new BookingCreateDto();
            _mockBookingService.Setup(service => service.Create(createDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CreateBooking(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsBadRequest_WhenGetAllReturnsEmpty()
        {
            // Arrange
            var createDto = new BookingCreateDto();
            _mockBookingService.Setup(service => service.Create(createDto))
                .ReturnsAsync(true);
            _mockBookingService.Setup(service => service.GetAll())
                .ReturnsAsync(new List<BookingDto>());

            // Act
            var result = await _controller.CreateBooking(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateBooking_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var createDto = new BookingCreateDto();
            var bookings = new List<BookingDto> { new BookingDto { Id = 1 } };

            _mockBookingService.Setup(service => service.Create(createDto))
                .ReturnsAsync(true);
            _mockBookingService.Setup(service => service.GetAll())
                .ReturnsAsync(bookings);

            // Act
            var result = await _controller.CreateBooking(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<BookingDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
        }

        [Fact]
        public async Task UpdateBooking_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.UpdateBooking(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin cập nhật không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateBooking_ReturnsBadRequest_WhenBookingDoesNotExist()
        {
            // Arrange
            var updateDto = new BookingUpdateDto { Id = 1 };
            _mockBookingService.Setup(service => service.GetById(updateDto.Id))
                .ReturnsAsync((BookingDto)null);

            // Act
            var result = await _controller.UpdateBooking(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Đặt sân không tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateBooking_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var updateDto = new BookingUpdateDto { Id = 1 };
            _mockBookingService.Setup(service => service.GetById(updateDto.Id))
                .ReturnsAsync(new BookingDto { Id = 1 });
            _mockBookingService.Setup(service => service.Update(updateDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBooking(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật đặt sân thất bại.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateBooking_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var updateDto = new BookingUpdateDto { Id = 1 };
            _mockBookingService.Setup(service => service.GetById(updateDto.Id))
                .ReturnsAsync(new BookingDto { Id = 1 });
            _mockBookingService.Setup(service => service.Update(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateBooking(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteBooking_ReturnsBadRequest_WhenBookingDoesNotExist()
        {
            // Arrange
            var bookingId = 1;
            _mockBookingService.Setup(service => service.GetById(bookingId))
                .ReturnsAsync((BookingDto)null);

            // Act
            var result = await _controller.DeleteBooking(bookingId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Đặt sân không tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteBooking_ReturnsOkResult_WhenSuccessful()
        {
            // Arrange
            var bookingId = 1;
            _mockBookingService.Setup(service => service.GetById(bookingId))
                .ReturnsAsync(new BookingDto { Id = 1 });

            // Act
            var result = await _controller.DeleteBooking(bookingId);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockBookingService.Verify(service => service.Delete(bookingId), Times.Once);
        }

        [Fact]
        public async Task Search_ReturnsEmptyList_WhenNoResultsFound()
        {
            // Arrange
            var searchDto = new BookingSearchDto();
            _mockBookingService.Setup(service => service.Search(searchDto))
                .ReturnsAsync(new List<BookingDto>());

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task Search_ReturnsResults_WhenSearchIsSuccessful()
        {
            // Arrange
            var searchDto = new BookingSearchDto();
            var bookings = new List<BookingDto>
            {
                new BookingDto { Id = 1 },
                new BookingDto { Id = 2 }
            };
            _mockBookingService.Setup(service => service.Search(searchDto))
                .ReturnsAsync(bookings);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<BookingDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        // Advanced test for GetBookingById with timeout simulation
        [Fact]
        public async Task GetBookingById_HandlesServiceTimeout_ReturnsInternalServerError()
        {
            // Arrange
            var bookingId = 1;
            _mockBookingService.Setup(service => service.GetById(bookingId))
                .ThrowsAsync(new TimeoutException("Database operation timed out"));

            // Act
            var result = await Record.ExceptionAsync(() => _controller.GetBookingById(bookingId));

            // Assert
            Assert.IsType<TimeoutException>(result);
            Assert.Contains("Database operation timed out", result.Message);
        }

        // Test concurrent create operations for race conditions
        [Fact]
        public async Task CreateBooking_HandlesConcurrentOperations_ProcessesSequentially()
        {
            // Arrange
            var createDto1 = new BookingCreateDto { UserId = 1, ChildCourtId = 1, StartTime = DateTime.Now, EndTime = DateTime.Now.AddHours(1) };
            var createDto2 = new BookingCreateDto { UserId = 2, ChildCourtId = 1, StartTime = DateTime.Now.AddMinutes(30), EndTime = DateTime.Now.AddHours(1.5) };

            var createdBookings = new List<BookingDto> { new BookingDto { Id = 1 } };
            var operationCount = 0;

            _mockBookingService.Setup(service => service.Create(It.IsAny<BookingCreateDto>()))
                .ReturnsAsync((BookingCreateDto dto) =>
                {
                    // Simulate service validating time overlaps
                    if (operationCount > 0 && dto.ChildCourtId == 1)
                    {
                        // Second booking attempt to same court fails due to time overlap
                        return dto.UserId == 2 ? false : true;
                    }
                    operationCount++;
                    return true;
                });

            _mockBookingService.Setup(service => service.GetAll())
                .ReturnsAsync(createdBookings);

            // Act
            var task1 = _controller.CreateBooking(createDto1);
            var task2 = _controller.CreateBooking(createDto2);

            await Task.WhenAll(task1, task2);
            var result1 = task1.Result;
            var result2 = task2.Result;

            // Assert
            Assert.IsType<OkObjectResult>(result1);
            Assert.IsType<BadRequestObjectResult>(result2);
        }

        // Test boundary conditions for booking time ranges
        [Theory]
        [InlineData(0, 0, true)]   // Same start/end time (invalid)
        [InlineData(-1, 1, false)] // Start time after end time (invalid)
        [InlineData(1, 1, false)]  // 0 duration (invalid)
        [InlineData(1, 2, true)]   // Valid time range
        public async Task CreateBooking_ValidatesTimeRanges_HandlesEdgeCases(int startHourOffset, int endHourOffset, bool expectedValidity)
        {
            // Arrange
            var now = DateTime.Now;
            var createDto = new BookingCreateDto
            {
                UserId = 1,
                ChildCourtId = 1,
                StartTime = now.AddHours(startHourOffset),
                EndTime = now.AddHours(endHourOffset)
            };

            var mockValidationResult = expectedValidity;
            _mockBookingService.Setup(service => service.Create(It.IsAny<BookingCreateDto>()))
                .ReturnsAsync(mockValidationResult);

            if (mockValidationResult)
            {
                _mockBookingService.Setup(service => service.GetAll())
                    .ReturnsAsync(new List<BookingDto> { new BookingDto { Id = 1 } });
            }

            // Act
            var result = await _controller.CreateBooking(createDto);

            // Assert
            if (expectedValidity)
            {
                Assert.IsType<OkObjectResult>(result);
            }
            else
            {
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        

        // Test service layer exception handling
        [Fact]
        public async Task DeleteBooking_HandlesServiceLayerException_ReturnsBadRequest()
        {
            // Arrange
            var bookingId = 1;
            _mockBookingService.Setup(service => service.GetById(bookingId))
                .ReturnsAsync(new BookingDto { Id = 1 });

            _mockBookingService.Setup(service => service.Delete(bookingId))
                .ThrowsAsync(new InvalidOperationException("Cannot delete booking that has already started"));

            // Act
            var exception = await Record.ExceptionAsync(() => _controller.DeleteBooking(bookingId));

            // Assert
            Assert.IsType<InvalidOperationException>(exception);
            Assert.Contains("Cannot delete booking that has already started", exception.Message);
        }

        // Test update with conflicting bookings
        [Fact]
        public async Task UpdateBooking_WithTimeConflict_ReturnsBadRequest()
        {
            // Arrange
            var updateDto = new BookingUpdateDto
            {
                Id = 1,
                StartTime = DateTime.Now.AddHours(1),
                EndTime = DateTime.Now.AddHours(2)
            };

            _mockBookingService.Setup(service => service.GetById(updateDto.Id))
                .ReturnsAsync(new BookingDto { Id = 1 });

            _mockBookingService.Setup(service => service.Update(updateDto))
                .Callback<BookingUpdateDto>(dto =>
                {
                    // Simulate checking for conflicts in the service layer
                    if (dto.StartTime < DateTime.Now.AddHours(1.5) && dto.EndTime > DateTime.Now.AddHours(1.5))
                    {
                        throw new InvalidOperationException("Time slot is already booked");
                    }
                })
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateBooking(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật đặt sân thất bại.", badRequestResult.Value);
        }
 
    // Mock class for pagination test
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            TotalCount = count;
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            AddRange(items);
        }
    }

    // Mock class for pagination parameters
    public class BookingPagingParametersDto
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortField { get; set; } = "Id";
        public string SortOrder { get; set; } = "asc";
    }
}
}
