using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SportRentHub.Controllers;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Payment;
using SportRentHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class UTPayment
    {
        private readonly Mock<IServiceManager> _mockServiceManager;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly PaymentController _controller;

        public UTPayment()
        { 
            _mockServiceManager = new Mock<IServiceManager>();
            _mockPaymentService = new Mock<IPaymentService>();

            // Setup service manager to return payment service
            _mockServiceManager.Setup(s => s.PaymentService).Returns(_mockPaymentService.Object);

            _controller = new PaymentController(_mockServiceManager.Object, _mockConfiguration.Object);
        }

        #region GetPaymentById Tests

        // Test 1: Cơ bản - Trả về Ok khi tìm thấy thanh toán theo ID
        [Fact]
        public async Task GetPaymentById_ReturnsOk_WhenPaymentExists()
        {
            // Arrange
            int paymentId = 1;
            var paymentDto = new PaymentDto { Id = paymentId };
            _mockPaymentService.Setup(s => s.GetById(paymentId)).ReturnsAsync(paymentDto);

            // Act
            var result = await _controller.GetPaymentById(paymentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaymentDto>(okResult.Value);
            Assert.Equal(paymentId, returnValue.Id);
        }

        // Test 2: Cơ bản - Trả về NoContent khi không tìm thấy thanh toán
        [Fact]
        public async Task GetPaymentById_ReturnsNoContent_WhenPaymentNotFound()
        {
            // Arrange
            int paymentId = 999;
            _mockPaymentService.Setup(s => s.GetById(paymentId)).ReturnsAsync((PaymentDto)null);

            // Act
            var result = await _controller.GetPaymentById(paymentId);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        #endregion

        #region GetPayments Tests

        // Test 3: Cơ bản - Trả về danh sách thanh toán khi có dữ liệu
        [Fact]
        public async Task GetPayments_ReturnsOkWithList_WhenPaymentsExist()
        {
            // Arrange
            var payments = new List<PaymentDto>
            {
                new PaymentDto { Id = 1 },
                new PaymentDto { Id = 2 }
            };
            _mockPaymentService.Setup(s => s.GetAll()).ReturnsAsync(payments);

            // Act
            var result = await _controller.GetPayments();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        // Test 4: Cơ bản - Trả về danh sách rỗng khi không có dữ liệu
        [Fact]
        public async Task GetPayments_ReturnsEmptyList_WhenNoPaymentsExist()
        {
            // Arrange
            _mockPaymentService.Setup(s => s.GetAll()).ReturnsAsync((List<PaymentDto>)null);

            // Act
            var result = await _controller.GetPayments();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        #endregion

        #region CreatePayment Tests

        // Test 5: Cơ bản - Trả về BadRequest khi dữ liệu tạo thanh toán là null
        [Fact]
        public async Task CreatePayment_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.CreatePayment(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin thanh toán không hợp lệ.", badRequestResult.Value);
        }

        // Test 6: Trung bình - Trả về BadRequest khi tạo thanh toán thất bại
        [Fact]
        public async Task CreatePayment_ReturnsBadRequest_WhenCreationFails()
        {
            // Arrange
            var createDto = new PaymentCreateDto();
            _mockPaymentService.Setup(s => s.Create(createDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreatePayment(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        // Test 7: Trung bình - Trả về BadRequest khi không lấy được danh sách sau khi tạo
        [Fact]
        public async Task CreatePayment_ReturnsBadRequest_WhenGetAllAfterCreateReturnsEmpty()
        {
            // Arrange
            var createDto = new PaymentCreateDto();
            _mockPaymentService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockPaymentService.Setup(s => s.GetAll()).ReturnsAsync(new List<PaymentDto>());

            // Act
            var result = await _controller.CreatePayment(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        // Test 8: Nâng cao - Trả về Ok với thanh toán đầu tiên khi tạo thành công
        [Fact]
        public async Task CreatePayment_ReturnsOkWithFirstPayment_WhenCreationSucceeds()
        {
            // Arrange
            var createDto = new PaymentCreateDto();
            var payments = new List<PaymentDto>
    {
        new PaymentDto {
            Id = 1,
            UserId = 101,
            BookingId = 201,
            CreateDate = DateTime.Now,
            Price = 150.75f,
            Type = 2,
            UserFullname = "Nguyễn Văn A",
            UserPhoneNumber = "0987654321",
            CourtId = 301,
            CourtName = "Sân Tennis Số 5"
        }
    };

            _mockPaymentService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockPaymentService.Setup(s => s.GetAll()).ReturnsAsync(payments);

            // Act
            var result = await _controller.CreatePayment(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaymentDto>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal(101, returnValue.UserId);
            Assert.Equal(201, returnValue.BookingId);
            Assert.Equal(150.75f, returnValue.Price);
            Assert.Equal(2, returnValue.Type);
            Assert.Equal("Nguyễn Văn A", returnValue.UserFullname);
            Assert.Equal("0987654321", returnValue.UserPhoneNumber);
            Assert.Equal(301, returnValue.CourtId);
            Assert.Equal("Sân Tennis Số 5", returnValue.CourtName);
        }

        #endregion

        #region UpdatePayment Tests

        // Test 9: Cơ bản - Trả về BadRequest khi dữ liệu cập nhật là null
        [Fact]
        public async Task UpdatePayment_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Act
            var result = await _controller.UpdatePayment(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin cập nhật không hợp lệ.", badRequestResult.Value);
        }

        // Test 10: Trung bình - Trả về BadRequest khi thanh toán không tồn tại
        [Fact]
        public async Task UpdatePayment_ReturnsBadRequest_WhenPaymentDoesNotExist()
        {
            // Arrange
            var updateDto = new PaymentUpdateDto { Id = 999 };
            _mockPaymentService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync((PaymentDto)null);

            // Act
            var result = await _controller.UpdatePayment(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thanh toán không tồn tại.", badRequestResult.Value);
        }

        // Test 11: Trung bình - Trả về BadRequest khi cập nhật thất bại
        [Fact]
        public async Task UpdatePayment_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var updateDto = new PaymentUpdateDto { Id = 1 };
            var existingPayment = new PaymentDto { Id = 1 };

            _mockPaymentService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(existingPayment);
            _mockPaymentService.Setup(s => s.Update(updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdatePayment(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật thanh toán thất bại.", badRequestResult.Value);
        }

        // Test 12: Nâng cao - Trả về Ok khi cập nhật thành công
        [Fact]
        public async Task UpdatePayment_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var updateDto = new PaymentUpdateDto { Id = 1 };
            var existingPayment = new PaymentDto { Id = 1 };

            _mockPaymentService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(existingPayment);
            _mockPaymentService.Setup(s => s.Update(updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePayment(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        #endregion

        #region DeletePayment Tests

        // Test 13: Cơ bản - Trả về BadRequest khi thanh toán không tồn tại
        [Fact]
        public async Task DeletePayment_ReturnsBadRequest_WhenPaymentDoesNotExist()
        {
            // Arrange
            int paymentId = 999;
            _mockPaymentService.Setup(s => s.GetById(paymentId)).ReturnsAsync((PaymentDto)null);

            // Act
            var result = await _controller.DeletePayment(paymentId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thanh toán không tồn tại.", badRequestResult.Value);
        }

        // Test 14: Nâng cao - Trả về Ok khi xóa thành công
        [Fact]
        public async Task DeletePayment_ReturnsOk_WhenDeleteSucceeds()
        {
            // Arrange
            int paymentId = 1;
            var existingPayment = new PaymentDto { Id = paymentId };

            _mockPaymentService.Setup(s => s.GetById(paymentId)).ReturnsAsync(existingPayment);
            _mockPaymentService.Setup(s => s.Delete(paymentId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeletePayment(paymentId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        // Test 15: Nâng cao - Xác nhận Delete được gọi với đúng ID
        [Fact]
        public async Task DeletePayment_CallsDeleteWithCorrectId_WhenPaymentExists()
        {
            // Arrange
            int paymentId = 1;
            var existingPayment = new PaymentDto { Id = paymentId };

            _mockPaymentService.Setup(s => s.GetById(paymentId)).ReturnsAsync(existingPayment);
            _mockPaymentService.Setup(s => s.Delete(It.IsAny<int>())).ReturnsAsync(true);

            // Act
            await _controller.DeletePayment(paymentId);

            // Assert
            _mockPaymentService.Verify(s => s.Delete(paymentId), Times.Once);
        }

        #endregion

        #region Search Tests

        // Test 16: Cơ bản - Trả về danh sách rỗng khi không tìm thấy kết quả
        [Fact]
        public async Task Search_ReturnsEmptyList_WhenNoResultsFound()
        {
            // Arrange
            var searchDto = new PaymentSearchDto();
            var emptyList = new List<PaymentDto>();

            _mockPaymentService.Setup(s => s.Search(searchDto)).ReturnsAsync(emptyList);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentDto>>(okResult.Value);
            Assert.Empty(returnValue);
        }

        // Test 17: Trung bình - Trả về danh sách kết quả khi tìm thấy
        [Fact]
        public async Task Search_ReturnsResults_WhenMatchesFound()
        {
            // Arrange
            var searchDto = new PaymentSearchDto();
            var searchResults = new List<PaymentDto>
            {
                new PaymentDto { Id = 1 },
                new PaymentDto { Id = 2 }
            };

            _mockPaymentService.Setup(s => s.Search(searchDto)).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentDto>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        #endregion

        #region Advanced Tests

        // Test 18: Nâng cao - Kiểm tra gọi đúng phương thức dịch vụ khi tạo thanh toán
        [Fact]
        public async Task CreatePayment_CallsServiceWithCorrectDto()
        {
            // Arrange
            var createDto = new PaymentCreateDto
            {
                UserId = 123,
                BookingId = 456,
                Price = 500.50f,
                Type = 1
            };

            _mockPaymentService.Setup(s => s.Create(It.IsAny<PaymentCreateDto>())).ReturnsAsync(true);
            _mockPaymentService.Setup(s => s.GetAll()).ReturnsAsync(new List<PaymentDto> { new PaymentDto() });

            // Act
            await _controller.CreatePayment(createDto);

            // Assert
            _mockPaymentService.Verify(s => s.Create(It.Is<PaymentCreateDto>(
                dto => dto.UserId == 123 &&
                       dto.BookingId == 456 &&
                       dto.Price == 500.50f &&
                       dto.Type == 1)),
                Times.Once);
        }

        // Test 19: Nâng cao - Kiểm tra xử lý ngoại lệ khi dịch vụ gây ra ngoại lệ
        [Fact]
        public async Task GetPaymentById_HandlesException_WhenServiceThrows()
        {
            // Arrange
            int paymentId = 1;
            _mockPaymentService.Setup(s => s.GetById(paymentId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetPaymentById(paymentId));
        }

        // Test 20: Nâng cao - Kiểm tra thứ tự gọi các phương thức khi cập nhật
        [Fact]
        public async Task UpdatePayment_CallsMethodsInCorrectOrder()
        {
            // Arrange
            var updateDto = new PaymentUpdateDto { Id = 1 };
            var existingPayment = new PaymentDto { Id = 1 };
            var sequence = new MockSequence();

            _mockPaymentService.InSequence(sequence)
                .Setup(s => s.GetById(updateDto.Id))
                .ReturnsAsync(existingPayment);

            _mockPaymentService.InSequence(sequence)
                .Setup(s => s.Update(updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdatePayment(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
            _mockPaymentService.Verify(s => s.GetById(updateDto.Id), Times.Once);
            _mockPaymentService.Verify(s => s.Update(updateDto), Times.Once);
        }

        #endregion
    }
}
