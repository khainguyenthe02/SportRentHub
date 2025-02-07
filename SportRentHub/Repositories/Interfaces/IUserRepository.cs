using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Models;

namespace SportRentHub.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> Create(User user);
        Task<List<User>> GetAll();
        Task<bool> Update(User user);
        Task<bool> Delete(int id);
        Task<User> GetById(int id);
        Task<List<User>> Search(UserSearchDto search);
        Task<User> GetByUsername(string username);
        Task<User> Login(string email, string password);
    }
}
