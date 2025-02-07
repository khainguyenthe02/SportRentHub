using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Models;

namespace SportRentHub.Services.Interfaces
{
    public interface IUserService
    {
        Task<bool> Create(UserCreateDto create);
        Task<List<UserDto>> GetAll();
        Task<bool> Delete(int id);
        Task<UserDto> GetById(int id);
        Task<bool> Update(UserUpdateDto update);
        Task<List<UserDto>> Search(UserSearchDto search);
        Task<UserDto> Login(AuthRequestDto loginRequest);
    }
}
