using Castle.Core.Configuration;
using Moq;
using SportRentHub.Entities.DTOs.ChildCourt;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories;
using SportRentHub.SqlDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Unit_Test_Repository
{
    public class ChildCourtRepositoryTests
    {
        private readonly Mock<IDbServices> _mockDbServices;
        private readonly ChildCourtRepository _repository;

        public ChildCourtRepositoryTests()
        {
            // Set up mock configuration
            var mockConfiguration = new Mock<IConfiguration>();

            // Initialize repository with mocked services
            _mockDbServices = new Mock<IDbServices>();

            // Using reflection to inject mock service instead of real one
            _repository = new ChildCourtRepository(mockConfiguration.Object);
            var field = typeof(ChildCourtRepository).GetField("_dbService", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field.SetValue(_repository, _mockDbServices.Object);
        }

        [Fact]
        public async Task Create_ShouldReturnTrue_WhenDbServiceReturnsPositiveNumber()
        {
            // Arrange
            var childCourt = new ChildCourt
            {
                CourtId = 1,
                ChildCourtName = "Test Court",
                ChildCourtDescription = "Test Description",
                Position = "A1",
                RentCost = 100,
                FixedRentCost = 50
            };

            _mockDbServices.Setup(s => s.EditData(
                It.IsAny<string>(),
                It.Is<ChildCourt>(c => c.CourtId == childCourt.CourtId && c.ChildCourtName == childCourt.ChildCourtName)))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.Create(childCourt);

            // Assert
            Assert.True(result);
            _mockDbServices.Verify(s => s.EditData(
                It.Is<string>(sql => sql.StartsWith("INSERT INTO tbl_child_court")),
                It.IsAny<ChildCourt>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldReturnFalse_WhenDbServiceReturnsZero()
        {
            // Arrange
            var childCourt = new ChildCourt
            {
                CourtId = 1,
                ChildCourtName = "Test Court"
            };

            _mockDbServices.Setup(s => s.EditData(
                It.IsAny<string>(),
                It.IsAny<ChildCourt>()))
                .ReturnsAsync(0);

            // Act
            var result = await _repository.Create(childCourt);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Delete_ShouldReturnTrue_WhenDbServiceReturnsPositiveNumber()
        {
            // Arrange
            int id = 5;

            _mockDbServices.Setup(s => s.EditData(
                It.IsAny<string>(),
                It.Is<object>(o => o.GetType().GetProperty("Id").GetValue(o).Equals(id))))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.Delete(id);

            // Assert
            Assert.True(result);
            _mockDbServices.Verify(s => s.EditData(
                It.Is<string>(sql => sql.Contains("DELETE FROM tbl_child_court")),
                It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldReturnFalse_WhenDbServiceReturnsZero()
        {
            // Arrange
            int id = 5;

            _mockDbServices.Setup(s => s.EditData(
                It.IsAny<string>(),
                It.IsAny<object>()))
                .ReturnsAsync(0);

            // Act
            var result = await _repository.Delete(id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetAll_ShouldReturnListOfChildCourts()
        {
            // Arrange
            var expectedCourts = new List<ChildCourt>
            {
                new ChildCourt { Id = 1, ChildCourtName = "Court 1" },
                new ChildCourt { Id = 2, ChildCourtName = "Court 2" }
            };

            _mockDbServices.Setup(s => s.GetAll<ChildCourt>(
                It.IsAny<string>(),
                It.IsAny<object>()))
                .ReturnsAsync(expectedCourts);

            // Act
            var result = await _repository.GetAll();

            // Assert
            Assert.Equal(expectedCourts.Count, result.Count);
            Assert.Equal(expectedCourts[0].Id, result[0].Id);
            Assert.Equal(expectedCourts[1].ChildCourtName, result[1].ChildCourtName);
            _mockDbServices.Verify(s => s.GetAll<ChildCourt>(
                It.Is<string>(sql => sql.Contains("SELECT * FROM tbl_child_court")),
                It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnChildCourt_WhenIdExists()
        {
            // Arrange
            int id = 1;
            var expectedCourt = new ChildCourt
            {
                Id = id,
                CourtId = 5,
                ChildCourtName = "Test Court",
                ChildCourtDescription = "Description",
                Position = "B2",
                RentCost = 200,
                FixedRentCost = 100
            };

            _mockDbServices.Setup(s => s.GetAsync<ChildCourt>(
                It.IsAny<string>(),
                It.Is<object>(o => o.GetType().GetProperty("Id").GetValue(o).Equals(id))))
                .ReturnsAsync(expectedCourt);

            // Act
            var result = await _repository.GetById(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCourt.Id, result.Id);
            Assert.Equal(expectedCourt.ChildCourtName, result.ChildCourtName);
            _mockDbServices.Verify(s => s.GetAsync<ChildCourt>(
                It.Is<string>(sql => sql.Contains("WHERE id = @Id")),
                It.IsAny<object>()), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldIncludeCorrectWhereClause_WhenSearchCriteriaProvided()
        {
            // Arrange
            var searchDto = new ChildCourtSearchDto
            {
                IdLst = new List<int> { 1, 2, 3 },
                Id = 1,
                CourtId = 5,
                ChildCourtName = "Test Court"
            };

            var expectedCourts = new List<ChildCourt>
            {
                new ChildCourt { Id = 1, ChildCourtName = "Test Court" }
            };

            _mockDbServices.Setup(s => s.GetAll<ChildCourt>(
                It.IsAny<string>(),
                It.IsAny<ChildCourtSearchDto>()))
                .ReturnsAsync(expectedCourts);

            // Act
            var result = await _repository.Search(searchDto);

            // Assert
            Assert.Single(result);
            Assert.Equal(expectedCourts[0].Id, result[0].Id);
            _mockDbServices.Verify(s => s.GetAll<ChildCourt>(
                It.Is<string>(sql =>
                    sql.Contains("WHERE 1=1") &&
                    sql.Contains("id IN @IdLst") &&
                    sql.Contains("id = @Id") &&
                    sql.Contains("court_id = @CourtId") &&
                    sql.Contains("child_court_name = @ChildCourtName")),
                It.IsAny<ChildCourtSearchDto>()), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldExcludeWhereClauses_WhenSearchCriteriaNotProvided()
        {
            // Arrange
            var searchDto = new ChildCourtSearchDto();

            var expectedCourts = new List<ChildCourt>
            {
                new ChildCourt { Id = 1, ChildCourtName = "Court 1" },
                new ChildCourt { Id = 2, ChildCourtName = "Court 2" }
            };

            _mockDbServices.Setup(s => s.GetAll<ChildCourt>(
                It.IsAny<string>(),
                It.IsAny<ChildCourtSearchDto>()))
                .ReturnsAsync(expectedCourts);

            // Act
            var result = await _repository.Search(searchDto);

            // Assert
            Assert.Equal(2, result.Count);
            _mockDbServices.Verify(s => s.GetAll<ChildCourt>(
                It.Is<string>(sql =>
                    sql.Contains("WHERE 1=1") &&
                    !sql.Contains("id IN @IdLst") &&
                    !sql.Contains("id = @Id") &&
                    !sql.Contains("court_id = @CourtId") &&
                    !sql.Contains("child_court_name = @ChildCourtName")),
                It.IsAny<ChildCourtSearchDto>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnTrue_WhenFieldsAreProvided()
        {
            // Arrange
            var childCourt = new ChildCourt
            {
                Id = 1,
                CourtId = 5,
                ChildCourtName = "Updated Court",
                ChildCourtDescription = "Updated Description",
                Position = "C3",
                RentCost = 300,
                FixedRentCost = 150
            };

            _mockDbServices.Setup(s => s.EditData(
                It.IsAny<string>(),
                It.IsAny<ChildCourt>()))
                .ReturnsAsync(1);

            // Act
            var result = await _repository.Update(childCourt);

            // Assert
            Assert.True(result);
            _mockDbServices.Verify(s => s.EditData(
                It.Is<string>(sql =>
                    sql.Contains("UPDATE tbl_child_court SET") &&
                    sql.Contains("court_id = @CourtId") &&
                    sql.Contains("child_court_name = @ChildCourtName") &&
                    sql.Contains("child_court_description = @ChildCourtDescription") &&
                    sql.Contains("position = @Position") &&
                    sql.Contains("rent_cost = @RentCost") &&
                    sql.Contains("fixed_rent_cost = @FixedRentCost") &&
                    sql.Contains("WHERE id = @Id")),
                It.IsAny<ChildCourt>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenNoFieldsAreProvided()
        {
            // Arrange
            var childCourt = new ChildCourt
            {
                Id = 1,
                CourtId = 0,
                ChildCourtName = "",
                ChildCourtDescription = "",
                Position = "",
                RentCost = 0,
                FixedRentCost = 0
            };

            // Act
            var result = await _repository.Update(childCourt);

            // Assert
            Assert.False(result);
            _mockDbServices.Verify(s => s.EditData(
                It.IsAny<string>(),
                It.IsAny<ChildCourt>()), Times.Never);
        }

        [Fact]
        public async Task Update_ShouldReturnFalse_WhenDbServiceReturnsZero()
        {
            // Arrange
            var childCourt = new ChildCourt
            {
                Id = 1,
                CourtId = 5,
                ChildCourtName = "Updated Court"
            };

            _mockDbServices.Setup(s => s.EditData(
                It.IsAny<string>(),
                It.IsAny<ChildCourt>()))
                .ReturnsAsync(0);

            // Act
            var result = await _repository.Update(childCourt);

            // Assert
            Assert.False(result);
        }
    }
}
