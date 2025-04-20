using Microsoft.AspNetCore.Mvc;
using Moq;
using SportRentHub.Controllers;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.Court;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class UTChildCourt
    {
        private readonly Mock<IServiceManager> _mockServiceManager;
        private readonly Mock<IChildCourtService> _mockChildCourtService;
        private readonly Mock<ICourtService> _mockCourtService;
        private readonly ChildCourtController _controller;

        public UTChildCourt()
        {
            _mockChildCourtService = new Mock<IChildCourtService>();
            _mockCourtService = new Mock<ICourtService>();
            _mockServiceManager = new Mock<IServiceManager>();

            _mockServiceManager.Setup(sm => sm.ChildCourtService).Returns(_mockChildCourtService.Object);
            _mockServiceManager.Setup(sm => sm.CourtService).Returns(_mockCourtService.Object);

            _controller = new ChildCourtController(_mockServiceManager.Object);
        }

        [Fact]
        public async Task GetChildCourtById_ReturnsOkResult_WithChildCourt_WhenChildCourtExists()
        {
            // Arrange
            int childCourtId = 1;
            var childCourtDto = new ChildCourtDto { Id = childCourtId, ChildCourtName = "Test Child Court", CourtId = 1 };

            _mockChildCourtService.Setup(s => s.GetById(childCourtId)).ReturnsAsync(childCourtDto);

            // Act
            var result = await _controller.GetChildCourtById(childCourtId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChildCourt = Assert.IsType<ChildCourtDto>(okResult.Value);
            Assert.Equal(childCourtId, returnedChildCourt.Id);
            Assert.Equal("Test Child Court", returnedChildCourt.ChildCourtName);
            Assert.Equal(1, returnedChildCourt.CourtId);
        }

        [Fact]
        public async Task GetChildCourtById_ReturnsNoContent_WhenChildCourtDoesNotExist()
        {
            // Arrange
            int childCourtId = 1;
            _mockChildCourtService.Setup(s => s.GetById(childCourtId)).ReturnsAsync((ChildCourtDto)null);

            // Act
            var result = await _controller.GetChildCourtById(childCourtId);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetChildCourts_ReturnsOkResult_WithEmptyList_WhenNoChildCourtsExist()
        {
            // Arrange
            _mockChildCourtService.Setup(s => s.GetAll()).ReturnsAsync(new List<ChildCourtDto>());

            // Act
            var result = await _controller.GetChildCourts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var childCourts = Assert.IsType<List<ChildCourtDto>>(okResult.Value);
            Assert.Empty(childCourts);
        }

        [Fact]
        public async Task GetChildCourts_ReturnsOkResult_WithChildCourts_WhenChildCourtsExist()
        {
            // Arrange
            var childCourts = new List<ChildCourtDto>
            {
                new ChildCourtDto { Id = 1, ChildCourtName = "Child Court 1", CourtId = 1 },
                new ChildCourtDto { Id = 2, ChildCourtName = "Child Court 2", CourtId = 1 }
            };

            _mockChildCourtService.Setup(s => s.GetAll()).ReturnsAsync(childCourts);

            // Act
            var result = await _controller.GetChildCourts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChildCourts = Assert.IsType<List<ChildCourtDto>>(okResult.Value);
            Assert.Equal(2, returnedChildCourts.Count);
            Assert.Equal("Child Court 1", returnedChildCourts[0].ChildCourtName);
            Assert.Equal("Child Court 2", returnedChildCourts[1].ChildCourtName);
        }

        [Fact]
        public async Task CreateChildCourt_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange & Act
            var result = await _controller.CreateChildCourt(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin sân con không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateChildCourt_ReturnsBadRequest_WhenRentCostIsOutOfRange()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                ChildCourtName = "New Child Court",
                CourtId = 1,
                RentCost = 150 // Rent cost higher than MaxPrice
            };

            var courtDto = new CourtDto
            {
                Id = 1,
                CourtName = "Parent Court",
                MinPrice = 50,
                MaxPrice = 100
            };

            _mockCourtService.Setup(s => s.GetById(createDto.CourtId)).ReturnsAsync(courtDto);

            // Act
            var result = await _controller.CreateChildCourt(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(" Giá thuê cho sân con không nằm trong khoảng giá được thiết lập của sân chính.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateChildCourt_ReturnsBadRequest_WhenRentCostIsTooLow()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                ChildCourtName = "New Child Court",
                CourtId = 1,
                RentCost = 30 // Rent cost lower than MinPrice
            };

            var courtDto = new CourtDto
            {
                Id = 1,
                CourtName = "Parent Court",
                MinPrice = 50,
                MaxPrice = 100
            };

            _mockCourtService.Setup(s => s.GetById(createDto.CourtId)).ReturnsAsync(courtDto);

            // Act
            var result = await _controller.CreateChildCourt(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(" Giá thuê cho sân con không nằm trong khoảng giá được thiết lập của sân chính.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateChildCourt_ReturnsBadRequest_WhenCreateFails()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                ChildCourtName = "New Child Court",
                CourtId = 1,
                RentCost = 75 // Valid rent cost
            };

            var courtDto = new CourtDto
            {
                Id = 1,
                CourtName = "Parent Court",
                MinPrice = 50,
                MaxPrice = 100
            };

            _mockCourtService.Setup(s => s.GetById(createDto.CourtId)).ReturnsAsync(courtDto);
            _mockChildCourtService.Setup(s => s.Create(createDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateChildCourt(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateChildCourt_ReturnsBadRequest_WhenGetAllReturnsEmptyList()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                ChildCourtName = "New Child Court",
                CourtId = 1,
                RentCost = 75 // Valid rent cost
            };

            var courtDto = new CourtDto
            {
                Id = 1,
                CourtName = "Parent Court",
                MinPrice = 50,
                MaxPrice = 100
            };

            _mockCourtService.Setup(s => s.GetById(createDto.CourtId)).ReturnsAsync(courtDto);
            _mockChildCourtService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockChildCourtService.Setup(s => s.GetAll()).ReturnsAsync(new List<ChildCourtDto>());

            // Act
            var result = await _controller.CreateChildCourt(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateChildCourt_ReturnsOkResult_WithFirstChildCourt_WhenCreateSucceeds()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                ChildCourtName = "New Child Court",
                CourtId = 1,
                RentCost = 75 // Valid rent cost
            };

            var courtDto = new CourtDto
            {
                Id = 1,
                CourtName = "Parent Court",
                MinPrice = 50,
                MaxPrice = 100
            };

            var childCourts = new List<ChildCourtDto>
            {
                new ChildCourtDto { Id = 1, ChildCourtName = "New Child Court", CourtId = 1, RentCost = 75 }
            };

            _mockCourtService.Setup(s => s.GetById(createDto.CourtId)).ReturnsAsync(courtDto);
            _mockChildCourtService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockChildCourtService.Setup(s => s.GetAll()).ReturnsAsync(childCourts);

            // Act
            var result = await _controller.CreateChildCourt(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChildCourt = Assert.IsType<ChildCourtDto>(okResult.Value);
            Assert.Equal(1, returnedChildCourt.Id);
            Assert.Equal("New Child Court", returnedChildCourt.ChildCourtName);
            Assert.Equal(1, returnedChildCourt.CourtId);
            Assert.Equal(75, returnedChildCourt.RentCost);
        }

        [Fact]
        public async Task UpdateChildCourt_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange & Act
            var result = await _controller.UpdateChildCourt(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin cập nhật không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateChildCourt_ReturnsBadRequest_WhenChildCourtDoesNotExist()
        {
            // Arrange
            var updateDto = new ChildCourtUpdateDto { Id = 1, ChildCourtName = "Updated Child Court" };
            _mockChildCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync((ChildCourtDto)null);

            // Act
            var result = await _controller.UpdateChildCourt(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Sân con không tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateChildCourt_ReturnsBadRequest_WhenRentCostIsOutOfRange()
        {
            // Arrange
            var updateDto = new ChildCourtUpdateDto
            {
                Id = 1,
                ChildCourtName = "Updated Child Court",
                CourtId = 1,
                RentCost = 150 // Rent cost higher than MaxPrice
            };

            var childCourtDto = new ChildCourtDto { Id = 1, ChildCourtName = "Original Child Court", CourtId = 1 };
            var courtDto = new CourtDto { Id = 1, CourtName = "Parent Court", MinPrice = 50, MaxPrice = 100 };

            _mockChildCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(childCourtDto);
            _mockCourtService.Setup(s => s.GetById((int)updateDto.CourtId)).ReturnsAsync(courtDto);

            // Act
            var result = await _controller.UpdateChildCourt(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(" Giá thuê cho sân con không nằm trong khoảng giá được thiết lập của sân chính.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateChildCourt_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var updateDto = new ChildCourtUpdateDto
            {
                Id = 1,
                ChildCourtName = "Updated Child Court",
                CourtId = 1,
                RentCost = 75 // Valid rent cost
            };

            var childCourtDto = new ChildCourtDto { Id = 1, ChildCourtName = "Original Child Court", CourtId = 1 };
            var courtDto = new CourtDto { Id = 1, CourtName = "Parent Court", MinPrice = 50, MaxPrice = 100 };

            _mockChildCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(childCourtDto);
            _mockCourtService.Setup(s => s.GetById((int)updateDto.CourtId)).ReturnsAsync(courtDto);
            _mockChildCourtService.Setup(s => s.Update(updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateChildCourt(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật sân con thất bại.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateChildCourt_ReturnsOkResult_WhenUpdateSucceeds()
        {
            // Arrange
            var updateDto = new ChildCourtUpdateDto
            {
                Id = 1,
                ChildCourtName = "Updated Child Court",
                CourtId = 1,
                RentCost = 75 // Valid rent cost
            };

            var childCourtDto = new ChildCourtDto { Id = 1, ChildCourtName = "Original Child Court", CourtId = 1 };
            var courtDto = new CourtDto { Id = 1, CourtName = "Parent Court", MinPrice = 50, MaxPrice = 100 };

            _mockChildCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(childCourtDto);
            _mockCourtService.Setup(s => s.GetById((int)updateDto.CourtId)).ReturnsAsync(courtDto);
            _mockChildCourtService.Setup(s => s.Update(updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateChildCourt(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteChildCourt_ReturnsBadRequest_WhenChildCourtDoesNotExist()
        {
            // Arrange
            int childCourtId = 1;
            _mockChildCourtService.Setup(s => s.GetById(childCourtId)).ReturnsAsync((ChildCourtDto)null);

            // Act
            var result = await _controller.DeleteChildCourt(childCourtId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Sân con không tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteChildCourt_ReturnsOkResult_WhenDeleteSucceeds()
        {
            // Arrange
            int childCourtId = 1;
            var childCourtDto = new ChildCourtDto { Id = childCourtId, ChildCourtName = "Child Court to Delete", CourtId = 1 };

            _mockChildCourtService.Setup(s => s.GetById(childCourtId)).ReturnsAsync(childCourtDto);
            _mockChildCourtService.Setup(s => s.Delete(childCourtId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteChildCourt(childCourtId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithEmptyList_WhenNoResultsFound()
        {
            // Arrange
            var searchDto = new ChildCourtSearchDto { ChildCourtName = "NonExistentChildCourt" };
            _mockChildCourtService.Setup(s => s.Search(searchDto)).ReturnsAsync(new List<ChildCourtDto>());

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var childCourts = Assert.IsType<List<ChildCourtDto>>(okResult.Value);
            Assert.Empty(childCourts);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithResults_WhenSearchSucceeds()
        {
            // Arrange
            var searchDto = new ChildCourtSearchDto { ChildCourtName = "Test" };
            var searchResults = new List<ChildCourtDto>
            {
                new ChildCourtDto { Id = 1, ChildCourtName  = "Test Child Court 1", CourtId = 1 },
                new ChildCourtDto { Id = 2, ChildCourtName = "Test Child Court 2", CourtId = 1 }
            };

            _mockChildCourtService.Setup(s => s.Search(searchDto)).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedChildCourts = Assert.IsType<List<ChildCourtDto>>(okResult.Value);
            Assert.Equal(2, returnedChildCourts.Count);
            Assert.Equal("Test Child Court 1", returnedChildCourts[0].ChildCourtName);
            Assert.Equal("Test Child Court 2", returnedChildCourts[1].ChildCourtName);
        }
    }
}
