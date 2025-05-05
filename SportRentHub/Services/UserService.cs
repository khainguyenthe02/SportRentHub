using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Serilog;
using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Enum;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.Services.Interfaces;

namespace SportRentHub.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryManager _repositoryManager;
        public UserService(IRepositoryManager repositoryManager)
        {
            this._repositoryManager = repositoryManager;
        }
        public async Task<bool> Create(UserCreateDto create)
        {
            try
            {
                var user = create.Adapt<User>();
                user.CreateDate = DateTime.Now;
                user.LastLogin = DateTime.Now;
                // hash password + salt
                var salt = Utils.Convert.GenerateSalt();
                var hashedPassword = Utils.Convert.ComputeSHA256Hash(create.Password, salt);
                user.Password = hashedPassword;
                user.Salt = salt;
                user.Status = (int)AccountStatus.ACTIVE;
				var result = await _repositoryManager.UserRepository.Create(user);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi tạo mới:{@createDto}", create, ex.Message);
                throw;
            }
        }

        public async Task<bool> Delete(int id)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.Delete(id);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi xóa với ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<List<UserDto>> GetAll()
        {
            try
            {
                var result = await _repositoryManager.UserRepository.GetAll();
                var listDto = result.Adapt<List<UserDto>>();
                return await FilterData(listDto);
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi lấy danh sách.", ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetById(int id)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.GetById(id);               
                if (result != null)
                {
                    var dto = result?.Adapt<UserDto>();
                    return (await FilterData(new() { dto })).FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi lấy dữ liệu với ID: {Id}", id, ex.Message);
                throw;
            }
        }

        public async Task<UserDto> Login(AuthRequestDto loginRequest)
        {
            try
            {
                var user = await _repositoryManager.UserRepository.GetByUsername(loginRequest.Username);
                if (user != null)
                {
                    var hashedPassword = Utils.Convert.ComputeSHA256Hash(loginRequest.Password, user.Salt);
                    if (hashedPassword == user.Password)
                    {
                        user.LastLogin = DateTime.Now;
                        var result = await _repositoryManager.UserRepository.Update(user);
                        if (result)
                        {
                            var dto = user.Adapt<UserDto>();
                            if (dto != null)
                            {
                                return (await FilterData(new() { dto })).FirstOrDefault();
                            }
                            return null;
                        }
                    }
                }
                return null;

            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi đăng nhập: {username}", loginRequest.Username, ex.Message);
                throw;
            }
        }

        public async Task<List<UserDto>> Search(UserSearchDto search)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.Search(search);
                var resultDto = result.Adapt<List<UserDto>>();
                return await FilterData(resultDto);
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi tìm kiếm dữ liệu: {@Search}", search, ex.Message);
                throw;
            }
        }

        public async Task<bool> Update(UserUpdateDto update)
        {
            try
            {
                var existUser = await _repositoryManager.UserRepository.GetById(update.Id);
                if (existUser == null)
                {
                    Log.Warning("[UserService] Không tìm thấy nhân viên với ID: {EmployeeId}", update.Id);
                    return false;
                }

                if (!string.IsNullOrEmpty(update.Password))
                {
                    update.Password = Utils.Convert.ComputeSHA256Hash(update.Password, existUser.Salt);
                }
                update.Adapt(existUser);

                var result = await _repositoryManager.UserRepository.Update(existUser);
                return result;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi cập nhật: {@Update}", update, ex.Message);
                throw;
            }
        }
        public async Task<List<UserDto>> FilterData(List<UserDto> lst)
        {
            try
            {
                if (lst?.Count() > 0)
                {
                    foreach (var item in lst)
                    {
                        item.RoleName = Utils.Convert.ConvertRole(item.Role);
                    }
                }
                return lst;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi lấy danh sách lọc.", ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetByUsername(string username)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.GetByUsername(username);
                if (result != null)
                {
                    var dto = result?.Adapt<UserDto>();
                    return (await FilterData(new() { dto })).FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi lấy dữ liệu với username: {Id}", username, ex.Message);
                throw;
            }
        }

        public async Task<UserDto> GetByEmail(string email)
        {
            try
            {
                var result = await _repositoryManager.UserRepository.GetByEmail(email);
                if (result != null)
                {
                    var dto = result?.Adapt<UserDto>();
                    return (await FilterData(new() { dto })).FirstOrDefault();
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("[UserService] Lỗi khi lấy dữ liệu với email: {Id}", email, ex.Message);
                throw;
            }
        }
    }
}
