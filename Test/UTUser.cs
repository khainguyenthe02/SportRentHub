using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using SportRentHub.Controllers;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services;
using SportRentHub.Services.Interfaces;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

public class UsersControllerTests // Đổi tên class từ UniTest1 thành UsersControllerTests
{
	private readonly Mock<IUserService> _mockUserService;
	private readonly Mock<IServiceManager> _mockServiceManager;
	private readonly Mock<IConfiguration> _mockConfiguration;
	private readonly UserController _controller;

	public UsersControllerTests() // Tên constructor khớp với tên class
	{
		_mockUserService = new Mock<IUserService>();
		_mockServiceManager = new Mock<IServiceManager>();
		_mockServiceManager.Setup(sm => sm.UserService).Returns(_mockUserService.Object);

		// Mock IConfiguration
		_mockConfiguration = new Mock<IConfiguration>();

		// Nếu cần thiết, thiết lập giá trị cho IConfiguration (ví dụ: giá trị từ appsettings.json)
		// Ví dụ: _mockConfiguration.Setup(c => c["SomeKey"]).Returns("SomeValue");

		// Khởi tạo controller với cả IServiceManager và IConfiguration
		_controller = new UserController(_mockServiceManager.Object, _mockConfiguration.Object);
	}

    [Fact]
    public async Task testSaveUser_Success()
    {
        // Arrange
        var userCreate = new UserCreateDto { Email = "test@mail.com", Username = "tester", PhoneNumber = "0123456789", Password = "Test123!" };
        var createdUser = new UserDto { Email = userCreate.Email, Username = userCreate.Username };

        _mockUserService.Setup(s => s.GetByEmail(userCreate.Email)).ReturnsAsync((UserDto)null);
        _mockUserService.Setup(s => s.GetByUsername(userCreate.Username)).ReturnsAsync((UserDto)null);
        _mockUserService.Setup(s => s.Search(It.IsAny<UserSearchDto>())).ReturnsAsync(new List<UserDto>());
        _mockUserService.Setup(s => s.Create(userCreate)).ReturnsAsync(true);
        _mockUserService.Setup(s => s.GetAll()).ReturnsAsync(new List<UserDto> { createdUser });

        // Act
        var result = await _controller.CreateUser(userCreate);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var user = okResult.Value.Should().BeOfType<UserDto>().Subject;
        user.Email.Should().Be(userCreate.Email);
    }


    [Fact]
    public async Task testExistsByUserName_UserExists()
    {
        var username = "existingUser";
        _mockUserService.Setup(s => s.GetByUsername(username)).ReturnsAsync(new UserDto { Username = username });

        var result = await _controller.CreateUser(new UserCreateDto { Username = username });

        result.Should().BeOfType<BadRequestObjectResult>();
    }


    [Fact]
    public async Task testExistsByUserName_UserDoesNotExists()
    {
        var username = "newUser";
        _mockUserService.Setup(s => s.GetByUsername(username)).ReturnsAsync((UserDto)null);
        _mockUserService.Setup(s => s.Create(It.IsAny<UserCreateDto>())).ReturnsAsync(true);
        _mockUserService.Setup(s => s.GetAll()).ReturnsAsync(new List<UserDto> { new UserDto { Username = username } });

        var result = await _controller.CreateUser(new UserCreateDto { Username = username, Password = "ValidPass1!" });

        result.Should().BeOfType<OkObjectResult>();
    }


    [Fact]
    public async Task testExistsByEmail_EmailExists()
    {
        var email = "exists@mail.com";
        _mockUserService.Setup(s => s.GetByEmail(email)).ReturnsAsync(new UserDto { Email = email });

        var result = await _controller.CreateUser(new UserCreateDto { Email = email });

        result.Should().BeOfType<BadRequestObjectResult>();
    }


    [Fact]
    public async Task testExistsByEmail_EmailDoesNotExists()
    {
        var email = "new@mail.com";
        _mockUserService.Setup(s => s.GetByEmail(email)).ReturnsAsync((UserDto)null);
        _mockUserService.Setup(s => s.Create(It.IsAny<UserCreateDto>())).ReturnsAsync(true);
        _mockUserService.Setup(s => s.GetAll()).ReturnsAsync(new List<UserDto> { new UserDto { Email = email } });

        var result = await _controller.CreateUser(new UserCreateDto { Email = email, Password = "Test123!", Username = "abc" });

        result.Should().BeOfType<OkObjectResult>();
    }


    [Fact]
    public async Task testFindById_UserFound()
    {
        var id = 1;
        _mockUserService.Setup(s => s.GetById(id)).ReturnsAsync(new UserDto { Id = id });

        var result = await _controller.GetUserById(id);

        result.Should().BeOfType<OkObjectResult>();
    }


    [Fact]
    public async Task testFindById_UserNotFound()
    {
        var id = 2;
        _mockUserService.Setup(s => s.GetById(id)).ReturnsAsync((UserDto)null);

        var result = await _controller.GetUserById(id);

        result.Should().BeOfType<StatusCodeResult>().Which.StatusCode.Should().Be((int)HttpStatusCode.NoContent);
    }


    [Fact]
    public async Task TestFindByUserName_UserFound()
    {
        // Arrange
        var username = "tester";
        var userDto = new UserDto { Username = username };

        // Setup UserService mock
        _mockUserService.Setup(s => s.GetByUsername(username))
                        .ReturnsAsync(userDto);

        // Setup ServiceManager mock to return the UserService mock
        _mockServiceManager.Setup(sm => sm.UserService)
                           .Returns(_mockUserService.Object);

        // Act
        var result = await _mockServiceManager.Object.UserService.GetByUsername(username);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(username);
    }   
}