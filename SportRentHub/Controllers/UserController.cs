using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using ERBeeVisionMaster.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SportRentHub.Entities.Const;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public UserController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }
        [HttpGet("id/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userDto = await _serviceManager.UserService.GetById(id);
            if (userDto is null)
            {
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            return Ok(userDto);
        }
        [HttpGet("get-list")]
        public async Task<IActionResult> GetUsers()
        {
            List<UserDto> userDto;
            userDto = await _serviceManager.UserService.GetAll();
            if (userDto == null) userDto = new();
            return Ok(userDto);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto createUserDto)
        {
            // Kiểm tra email có trùng hay không
            if (createUserDto.Email != null)
            {
                if (!Validate.IsValidEmail(createUserDto.Email))
                {
                    return BadRequest(MessageError.TypeEmailError);
                }
                var userDto = await _serviceManager.UserService.GetByEmail(createUserDto.Email);
                if (userDto != null)
                {
                    return BadRequest(MessageError.EmailExist);
                }
                if (createUserDto.Password == null)
                {
                    return BadRequest(MessageError.InvalidPasswordError);
                }
                if (!Validate.ValidatePasword(createUserDto.Password))
                {
                    return BadRequest(MessageError.TypingPasswordError);
                }
                var result = await _serviceManager.UserService.Create(createUserDto);
                if (result)
                {
                    return Ok();
                }
            }
            return BadRequest(MessageError.ErrorCreate);
        }
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var userDto = await _serviceManager.UserService.GetById(id);
            if (userDto != null)
            {
                await _serviceManager.UserService.Delete(id);

                return Ok(); //NoContent();
            }
            return BadRequest("Người dùng không tồn tại");
        }
        [HttpPut("update")]
        //[Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updateUserDto, CancellationToken cancellationToken)
        {
            // Kiểm tra user có trùng hay không
            var userDto = await _serviceManager.UserService.GetById(updateUserDto.Id);
            if (userDto == null) return StatusCode((int)HttpStatusCode.BadRequest, "Người dùng không tồn tại");
            if (await _serviceManager.UserService.Update(updateUserDto))
            {
                return Ok();
            }
            return BadRequest(MessageError.ErrorUpdate);
        }
    }
}
