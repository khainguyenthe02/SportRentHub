using Mapster;
using Moq;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Unit_Test_Service
{
    public class ChildCourtServiceTests
    {
        private readonly Mock<IRepositoryManager> _mockRepoManager;
        private readonly Mock<IChildCourtRepository> _mockChildCourtRepo;
        private readonly ChildCourtService _service;

        public ChildCourtServiceTests()
        {
            _mockRepoManager = new Mock<IRepositoryManager>();
            _mockChildCourtRepo = new Mock<IChildCourtRepository>();

            _mockRepoManager.Setup(rm => rm.ChildCourtRepository)
                .Returns(_mockChildCourtRepo.Object);

            _service = new ChildCourtService(_mockRepoManager.Object);

            // Thiết lập Mapster TypeAdapterConfig nếu cần
            SetupMapsterConfig();
        }

        private void SetupMapsterConfig()
        {
            //TypeAdapterConfig.GlobalSettings = new TypeAdapterConfig();
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);

            TypeAdapterConfig<ChildCourtCreateDto, ChildCourt>.NewConfig();
            TypeAdapterConfig<ChildCourt, ChildCourtDto>.NewConfig();
            TypeAdapterConfig<ChildCourtUpdateDto, ChildCourt>.NewConfig()
                .IgnoreNullValues(true);
        }

        [Fact]
        public async Task GetAll_ReturnsAllChildCourts()
        {
            // Arrange
            var childCourts = new List<ChildCourt>
            {
                new ChildCourt { Id = 1, ChildCourtName = "Court 1", RentCost = 100000 },
                new ChildCourt { Id = 2, ChildCourtName = "Court 2", RentCost = 150000 }
            };

            _mockChildCourtRepo.Setup(repo => repo.GetAll())
                .ReturnsAsync(childCourts);

            // Act
            var result = await _service.GetAll();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Court 1", result[0].ChildCourtName);
            Assert.Equal(100000, result[0].RentCost);
        }

        [Fact]
        public async Task GetById_ReturnsChildCourt_WhenExists()
        {
            // Arrange
            int testId = 1;
            var childCourt = new ChildCourt
            {
                Id = testId,
                ChildCourtName = "Test Court",
                RentCost = 100000
            };

            _mockChildCourtRepo.Setup(repo => repo.GetById(testId))
                .ReturnsAsync(childCourt);

            // Act
            var result = await _service.GetById(testId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(testId, result.Id);
            Assert.Equal("Test Court", result.ChildCourtName);
            Assert.Equal(100000, result.RentCost);
        }

        [Fact]
        public async Task GetById_ReturnsNull_WhenNotExists()
        {
            // Arrange
            int testId = 999;

            _mockChildCourtRepo.Setup(repo => repo.GetById(testId))
                .ReturnsAsync((ChildCourt)null);

            // Act
            var result = await _service.GetById(testId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Create_ReturnsTrue_WhenCreateSucceeds()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                CourtId = 1,
                ChildCourtName = "New Court",
                RentCost = 100000
            };

            _mockChildCourtRepo.Setup(repo => repo.Create(It.IsAny<ChildCourt>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Create(createDto);

            // Assert
            Assert.True(result);
            _mockChildCourtRepo.Verify(repo => repo.Create(It.IsAny<ChildCourt>()), Times.Once);
        }

        [Fact]
        public async Task Create_PropagatesException_WhenErrorOccurs()
        {
            // Arrange
            var createDto = new ChildCourtCreateDto
            {
                CourtId = 1,
                ChildCourtName = "New Court",
                RentCost = 100000
            };

            _mockChildCourtRepo.Setup(repo => repo.Create(It.IsAny<ChildCourt>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.Create(createDto));
        }

        [Fact]
        public async Task Update_ReturnsFalse_WhenChildCourtNotFound()
        {
            // Arrange
            var updateDto = new ChildCourtUpdateDto
            {
                Id = 999,
                ChildCourtName = "Updated Court"
            };

            _mockChildCourtRepo.Setup(repo => repo.GetById(updateDto.Id))
                .ReturnsAsync((ChildCourt)null);

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.False(result);
            _mockChildCourtRepo.Verify(repo => repo.Update(It.IsAny<ChildCourt>()), Times.Never);
        }

        [Fact]
        public async Task Update_ReturnsTrue_WhenUpdateSucceeds()
        {
            // Arrange
            var updateDto = new ChildCourtUpdateDto
            {
                Id = 1,
                ChildCourtName = "Updated Court"
            };

            var existingChildCourt = new ChildCourt
            {
                Id = 1,
                ChildCourtName = "Original Court",
                RentCost = 100000
            };

            _mockChildCourtRepo.Setup(repo => repo.GetById(updateDto.Id))
                .ReturnsAsync(existingChildCourt);

            _mockChildCourtRepo.Setup(repo => repo.Update(It.IsAny<ChildCourt>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Update(updateDto);

            // Assert
            Assert.True(result);
            _mockChildCourtRepo.Verify(repo => repo.Update(It.IsAny<ChildCourt>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsTrue_WhenDeleteSucceeds()
        {
            // Arrange
            int testId = 1;

            _mockChildCourtRepo.Setup(repo => repo.Delete(testId))
                .ReturnsAsync(true);

            // Act
            var result = await _service.Delete(testId);

            // Assert
            Assert.True(result);
            _mockChildCourtRepo.Verify(repo => repo.Delete(testId), Times.Once);
        }

        [Fact]
        public async Task Search_ReturnsSearchResults()
        {
            // Arrange
            var searchDto = new ChildCourtSearchDto
            {
                ChildCourtName = "Court"
            };

            var searchResults = new List<ChildCourt>
            {
                new ChildCourt { Id = 1, ChildCourtName = "Court 1", RentCost = 100000 },
                new ChildCourt { Id = 2, ChildCourtName = "Court 2", RentCost = 150000 }
            };

            _mockChildCourtRepo.Setup(repo => repo.Search(searchDto))
                .ReturnsAsync(searchResults);

            // Act
            var result = await _service.Search(searchDto);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Id);
            Assert.Equal("Court 1", result[0].ChildCourtName);
        }
    }
}
