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
using SportRentHub.Entities.DTOs.Booking;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Extensions;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        private readonly IConfiguration _config;
        public UserController(IServiceManager serviceManager, IConfiguration configuration)
        {
            _serviceManager = serviceManager;
            _config = configuration;
        }
        [HttpGet("id/{id}")]
		[Authorize(Roles = "Admin, User")]
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
		[Authorize(Roles = "Admin, User" )]
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
                var userDto = await _serviceManager.UserService.GetByEmail(createUserDto.Email);
                if (userDto != null)
                {
                    return BadRequest(MessageError.EmailExist);
                }
            }
			if (createUserDto.Username != null)
			{
				var userDto = await _serviceManager.UserService.GetByUsername(createUserDto.Username);
				if (userDto != null)
				{
					return BadRequest("Username đã tồn tại");
				}
			}
			if (createUserDto.PhoneNumber != null)
			{
				if (createUserDto.PhoneNumber == null)
				{
					return BadRequest("Định dạng số điện thoại không hợp lệ");
				}
				var userByPhone = await _serviceManager.UserService.Search(new UserSearchDto { PhoneNumber = createUserDto.PhoneNumber});
				if (userByPhone.Any())
				{
					return BadRequest("Số điện thoại đã tồn tại");
				}
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
            if (!result) return BadRequest(MessageError.ErrorCreate);

            var lst = new List<UserDto>();
            lst = await _serviceManager.UserService.GetAll();
            if (!lst.Any()) return BadRequest(MessageError.ErrorCreate);
            return Ok(lst[0]);
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] AuthRequestDto loginRequest)
        {
            loginRequest.Username = loginRequest.Username.Replace(" ", "");
            loginRequest.Password = loginRequest.Password.Replace(" ", "");

			// check username
			UserDto user = new UserDto();
			if (Validate.IsValidPhoneNumber(loginRequest.Username))
			{
				user = (await _serviceManager.UserService.Search( new UserSearchDto { PhoneNumber = loginRequest.Username })).FirstOrDefault();
			}
			else
			{
				user = await _serviceManager.UserService.GetByUsername(loginRequest.Username);
			}
			if (user == null)
			{
				return BadRequest(MessageError.InvalidUser);
			}
			//login
			var userLogin = await _serviceManager.UserService.Login(loginRequest);
            if (userLogin == null)
            {
                return BadRequest(MessageError.LoginError);
            }
            // check token
            if (!string.IsNullOrEmpty(userLogin.Token))
            {
                var updateNullToken = new UserUpdateDto
                {
                    Id = userLogin.Id,
                    Token = "",
                };
                await _serviceManager.UserService.Update(updateNullToken);
            }

            //login successfully
            var claims = new[]
            {
                new Claim("email", userLogin.Email),
                new Claim("username",userLogin.Username),
                new Claim("userId", userLogin.Id.ToString()),
                new Claim(ClaimTypes.Role,userLogin.RoleName),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Issuer"],
                claims,
                expires: DateTime.Now.AddDays(4),
                signingCredentials: creds);
            string jwtToken = new JwtSecurityTokenHandler().WriteToken(token);
            var updateUser = new UserUpdateDto
            {
                Id = userLogin.Id,
                Token = jwtToken,
            };
            await _serviceManager.UserService.Update(updateUser);
            userLogin.Token = jwtToken;
            var result = new
            {
                token = jwtToken,
                infor = userLogin
            };
            return Ok(result);
        }
        [HttpDelete("delete")]
		[Authorize(Roles = "Admin")]
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
		[Authorize(Roles = "Admin, User")]
		//[Authorize]
		public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updateUserDto, CancellationToken cancellationToken)
        {
            // Kiểm tra user có trùng hay không
            var userDto = await _serviceManager.UserService.GetById(updateUserDto.Id);
            if (userDto == null) return BadRequest(MessageError.InvalidUser);
            if (updateUserDto.PhoneNumber != null)
            {
                var userExist = await _serviceManager.UserService.Search(new UserSearchDto { PhoneNumber = updateUserDto.PhoneNumber });
                if (userExist != null) return BadRequest("Số điện thoại này đã tồn tại");
            }
            if (updateUserDto.Email != null)
            {
                var userExist = await _serviceManager.UserService.Search(new UserSearchDto { Email = updateUserDto.Email });
                if (userExist != null) return BadRequest("Email này đã tồn tại");
            }
            if (await _serviceManager.UserService.Update(updateUserDto))
            {
                return Ok();
            }
            return BadRequest(MessageError.ErrorUpdate);
        }
        [HttpGet("Logout")]
        [Authorize()]
        public async Task<IActionResult> Logout()
        {
			var userId = User.FindFirstValue("userId");
			if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userID))
			{
				return BadRequest("Không thể tìm thấy người dùng");
			}
			var updateNullToken = new UserUpdateDto
			{
				Id = int.Parse(userId),
				Token = "",
			};
			await _serviceManager.UserService.Update(updateNullToken);
			return Ok("Đã đăng xuất thành công.");
		}
		[HttpPost("search")]
		[Authorize(Roles = "Admin, User")]
		public async Task<IActionResult> Search(UserSearchDto search)
		{
			var searchList = await _serviceManager.UserService.Search(search);
			if (!searchList.Any()) return Ok(searchList);
			return Ok(searchList);
		}
		[HttpPost("forgot-password")]
		[AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromForm] string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(MessageError.TypeEmailError);
            }

            var user = await _serviceManager.UserService.Search(new UserSearchDto { Email = email });
            if (user == null || !user.Any())
            {
                return BadRequest(MessageError.EmailNotExist);
            }

            var userDto = user.First();

            // Tạo JWT chứa thông tin reset password
            var claims = new[]
            {
                new Claim("email", userDto.Email.ToString()),
                new Claim("userId", userDto.Id.ToString())
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Tokens:Issuer"],
                audience: _config["Tokens:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(5),
                signingCredentials: creds
            );

            string resetToken = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine(resetToken);

            // Gửi email với liên kết reset password
            var feUrl = _config["Frontend:FE_URL_RESET"];
            string resetLink = $"{feUrl}reset-password?token={resetToken}";
            bool emailSent = await _serviceManager.EmailService.SendEmail(
                to: userDto.Email,
                subject: "Admin - Đặt lại mật khẩu tài khoản",
                body: $@"
                    <html>
                        <body>
                            <p>Chào {userDto.Fullname},</p>
                            <p>Bạn đã yêu cầu đặt lại mật khẩu cho username: <b>{userDto.Username}<b>. Nhấn vào liên kết dưới đây để đặt mật khẩu mới:</p>
                            <a href='{resetLink}'>Đặt lại</a>
                            <p>Liên kết này sẽ hết hạn sau 5 phút.</p>
                        </body>
                    </html>"
            );

            if (!emailSent)
            {
                return BadRequest(MessageError.EmailSendFailed);
            }

            return Ok("Đã gửi liên kết đặt lại mật khẩu đến email của bạn.");
        }
        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            if (string.IsNullOrEmpty(resetPassword.Token) || string.IsNullOrEmpty(resetPassword.NewPassword))
            {
                return BadRequest("Token hoặc mật khẩu mới không được để trống.");
            }

            // Kiểm tra tính hợp lệ của token
            bool isValid = JWTExtensions.IsTokenValid(resetPassword.Token, _config);
            if (!isValid)
            {
                return BadRequest(MessageError.InvalidToken);
            }
            // giải mã token
            var claimsPrincipal = JWTExtensions.DecodeJwtToken(resetPassword.Token, _config);

            // Lấy email và employeeId từ token
            var email = claimsPrincipal.Claims.FirstOrDefault(c =>
                c.Type == "email" || c.Type == ClaimTypes.Email)?.Value;
            var userId = claimsPrincipal.FindFirst("userId")?.Value;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(userId))
            {
                return BadRequest(MessageError.InvalidToken + " Không tìm thấy thông tin trong token");
            }

            // Kiểm tra người dùng
            var user = await _serviceManager.UserService.GetById(int.Parse(userId));
            if (user == null || !user.Email.Equals(email))
            {
                return BadRequest(MessageError.InvalidUser);
            }

            // Cập nhật mật khẩu

            var updateResult = await _serviceManager.UserService.Update(
                new UserUpdateDto
                {
                    Id = user.Id,
                    Password = resetPassword.NewPassword,
                    Token = ""
                });
            if (!updateResult)
            {
                return BadRequest(MessageError.ErrorUpdate);
            }

            return Ok("Mật khẩu đã được cập nhật thành công.");
        }
    }
}
