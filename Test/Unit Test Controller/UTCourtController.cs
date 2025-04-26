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
    public class UTCourtController
    {
        private readonly Mock<IServiceManager> _mockServiceManager;
        private readonly Mock<ICourtService> _mockCourtService;
        private readonly Mock<IChildCourtService> _mockChildCourtService;
        private readonly CourtController _controller;

        public UTCourtController()
        {
            _mockCourtService = new Mock<ICourtService>();
            _mockChildCourtService = new Mock<IChildCourtService>();
            _mockServiceManager = new Mock<IServiceManager>();

            _mockServiceManager.Setup(sm => sm.CourtService).Returns(_mockCourtService.Object);
            _mockServiceManager.Setup(sm => sm.ChildCourtService).Returns(_mockChildCourtService.Object);

            _controller = new CourtController(_mockServiceManager.Object);
        }

        [Fact]
        public async Task GetCourtById_ReturnsOkResult_WithCourt_WhenCourtExists()
        {
            // Arrange
            int courtId = 1;
            var courtDto = new CourtDto { Id = courtId, CourtName = "Test Court" };
            var childCourts = new List<ChildCourtDto>
            {
                new ChildCourtDto { Id = 1, CourtId = courtId, ChildCourtName = "Child Court 1" }
            };

            _mockCourtService.Setup(s => s.GetById(courtId)).ReturnsAsync(courtDto);
            _mockChildCourtService.Setup(s => s.Search(It.IsAny<ChildCourtSearchDto>())).ReturnsAsync(childCourts);

            // Act
            var result = await _controller.GetCourtById(courtId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCourt = Assert.IsType<CourtDto>(okResult.Value);
            Assert.Equal(courtId, returnedCourt.Id);
            Assert.Equal("Test Court", returnedCourt.CourtName);
            Assert.Equal(childCourts, returnedCourt.ChildLst);
        }

        [Fact]
        public async Task GetCourtById_ReturnsNoContent_WhenCourtDoesNotExist()
        {
            // Arrange
            int courtId = 1;
            _mockCourtService.Setup(s => s.GetById(courtId)).ReturnsAsync((CourtDto)null);

            // Act
            var result = await _controller.GetCourtById(courtId);

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetCourts_ReturnsOkResult_WithEmptyList_WhenNoCourtsExist()
        {
            // Arrange
            _mockCourtService.Setup(s => s.GetAll()).ReturnsAsync(new List<CourtDto>());

            // Act
            var result = await _controller.GetCourts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var courts = Assert.IsType<List<CourtDto>>(okResult.Value);
            Assert.Empty(courts);
        }

        [Fact]
        public async Task GetCourts_ReturnsOkResult_WithCourts_WhenCourtsExist()
        {
            // Arrange
            var courts = new List<CourtDto>
            {
                new CourtDto { Id = 1, CourtName = "Court 1" },
                new CourtDto { Id = 2, CourtName = "Court 2" }
            };

            var childCourts1 = new List<ChildCourtDto>
            {
                new ChildCourtDto { Id = 1, CourtId = 1, ChildCourtName = "Child Court 1" }
            };

            var childCourts2 = new List<ChildCourtDto>
            {
                new ChildCourtDto { Id = 2, CourtId = 2, ChildCourtName = "Child Court 2" }
            };

            _mockCourtService.Setup(s => s.GetAll()).ReturnsAsync(courts);
            _mockChildCourtService.Setup(s => s.Search(It.Is<ChildCourtSearchDto>(dto => dto.CourtId == 1))).ReturnsAsync(childCourts1);
            _mockChildCourtService.Setup(s => s.Search(It.Is<ChildCourtSearchDto>(dto => dto.CourtId == 2))).ReturnsAsync(childCourts2);

            // Act
            var result = await _controller.GetCourts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCourts = Assert.IsType<List<CourtDto>>(okResult.Value);
            Assert.Equal(2, returnedCourts.Count);
            Assert.Equal(childCourts1, returnedCourts[0].ChildLst);
            Assert.Equal(childCourts2, returnedCourts[1].ChildLst);
        }

        [Fact]
        public async Task CreateCourt_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange & Act
            var result = await _controller.CreateCourt(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin sân không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCourt_ReturnsBadRequest_WhenCreateFails()
        {
            // Arrange
            var createDto = new CourtCreateDto { CourtName = "New Court" };
            _mockCourtService.Setup(s => s.Create(createDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.CreateCourt(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCourt_ReturnsBadRequest_WhenGetAllReturnsEmptyList()
        {
            // Arrange
            var createDto = new CourtCreateDto { CourtName = "New Court" };
            _mockCourtService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockCourtService.Setup(s => s.GetAll()).ReturnsAsync(new List<CourtDto>());

            // Act
            var result = await _controller.CreateCourt(createDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(MessageError.ErrorCreate, badRequestResult.Value);
        }

        [Fact]
        public async Task CreateCourt_ReturnsOkResult_WithFirstCourt_WhenCreateSucceeds()
        {
            // Arrange
            var createDto = new CourtCreateDto { CourtName = "New Court" };
            var courts = new List<CourtDto>
            {
                new CourtDto { Id = 1, CourtName = "New Court" }
            };

            _mockCourtService.Setup(s => s.Create(createDto)).ReturnsAsync(true);
            _mockCourtService.Setup(s => s.GetAll()).ReturnsAsync(courts);

            // Act
            var result = await _controller.CreateCourt(createDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCourt = Assert.IsType<CourtDto>(okResult.Value);
            Assert.Equal(1, returnedCourt.Id);
            Assert.Equal("New Court", returnedCourt.CourtName);
        }

        [Fact]
        public async Task UpdateCourt_ReturnsBadRequest_WhenDtoIsNull()
        {
            // Arrange & Act
            var result = await _controller.UpdateCourt(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Thông tin cập nhật không hợp lệ.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCourt_ReturnsBadRequest_WhenCourtDoesNotExist()
        {
            // Arrange
            var updateDto = new CourtUpdateDto { Id = 1, CourtName = "Updated Court" };
            _mockCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync((CourtDto)null);

            // Act
            var result = await _controller.UpdateCourt(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Sân không tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCourt_ReturnsBadRequest_WhenUpdateFails()
        {
            // Arrange
            var updateDto = new CourtUpdateDto { Id = 1, CourtName = "Updated Court" };
            var courtDto = new CourtDto { Id = 1, CourtName = "Original Court" };

            _mockCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(courtDto);
            _mockCourtService.Setup(s => s.Update(updateDto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateCourt(updateDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cập nhật sân thất bại.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateCourt_ReturnsOkResult_WhenUpdateSucceeds()
        {
            // Arrange
            var updateDto = new CourtUpdateDto { Id = 1, CourtName = "Updated Court" };
            var courtDto = new CourtDto { Id = 1, CourtName = "Original Court" };

            _mockCourtService.Setup(s => s.GetById(updateDto.Id)).ReturnsAsync(courtDto);
            _mockCourtService.Setup(s => s.Update(updateDto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateCourt(updateDto);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task DeleteCourt_ReturnsBadRequest_WhenCourtDoesNotExist()
        {
            // Arrange
            int courtId = 1;
            _mockCourtService.Setup(s => s.GetById(courtId)).ReturnsAsync((CourtDto)null);

            // Act
            var result = await _controller.DeleteCourt(courtId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Sân không tồn tại.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteCourt_ReturnsOkResult_WhenDeleteSucceeds()
        {
            // Arrange
            int courtId = 1;
            var courtDto = new CourtDto { Id = 1, CourtName = "Court to Delete" };

            _mockCourtService.Setup(s => s.GetById(courtId)).ReturnsAsync(courtDto);
            _mockCourtService.Setup(s => s.Delete(courtId)).ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteCourt(courtId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithEmptyList_WhenNoResultsFound()
        {
            // Arrange
            var searchDto = new CourtSearchDto { CourtName = "NonExistentCourt" };
            _mockCourtService.Setup(s => s.Search(searchDto)).ReturnsAsync(new List<CourtDto>());

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var courts = Assert.IsType<List<CourtDto>>(okResult.Value);
            Assert.Empty(courts);
        }

        [Fact]
        public async Task Search_ReturnsOkResult_WithResults_WhenSearchSucceeds()
        {
            // Arrange
            var searchDto = new CourtSearchDto { CourtName = "Test" };
            var searchResults = new List<CourtDto>
            {
                new CourtDto { Id = 1, CourtName = "Test Court 1" },
                new CourtDto { Id = 2, CourtName = "Test Court 2" }
            };

            _mockCourtService.Setup(s => s.Search(searchDto)).ReturnsAsync(searchResults);

            // Act
            var result = await _controller.Search(searchDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCourts = Assert.IsType<List<CourtDto>>(okResult.Value);
            Assert.Equal(2, returnedCourts.Count);
            Assert.Equal("Test Court 1", returnedCourts[0].CourtName);
            Assert.Equal("Test Court 2", returnedCourts[1].CourtName);
        }
    }
}
