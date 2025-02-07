using SportRentHub.Entities.DTOs.User;
using SportRentHub.Entities.Models;
using SportRentHub.Repositories.Interfaces;
using SportRentHub.SqlDBHelper;

namespace SportRentHub.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbServices _dbService;

        public UserRepository(IConfiguration configuration)
        {
            _dbService = new DbServices(configuration);
        }

        public async Task<bool> Create(User user)
        {
            var result = await _dbService.EditData(
                "INSERT INTO tbl_user (username, password, fullname, phone_number, address, email, role, create_time, last_login) " +
                "VALUES (@Username, @Password, @Fullname, @PhoneNumber, @Address, @Email, @Role, @CreateTime, @LastLogin)",
                user);

            return result > 0;
        }

        public async Task<bool> Delete(int id)
        {
            var result = await _dbService.EditData(
                "DELETE FROM tbl_user WHERE id = @Id",
                new { Id = id }
            );
            return result > 0;
        }

        public async Task<List<User>> GetAll()
        {
            var userList = await _dbService.GetAll<User>(
                "SELECT * FROM tbl_user",
                new { }
            );
            return userList;
        }

        public async Task<User> GetById(int id)
        {
            var user = await _dbService.GetAsync<User>(
                "SELECT * FROM tbl_user WHERE id = @Id",
                new { Id = id }
            );
            return user;
        }

        public async Task<User> GetByUsername(string username)
        {
            User emp = new User();
            emp.Username = username;
            var selectSql = " SELECT * FROM tbl_user ";
            var whereSql = " WHERE 1=1 ";
            if (!string.IsNullOrEmpty(username))
            {
                whereSql += " AND username = @Username ";
            }
            var result = await _dbService.GetAsync<User>(selectSql + whereSql, emp);
            return result;
        }

        public async Task<User> Login(string email, string password)
        {
            var parameters = new { Email = email, Password = password };
            var selectSql = "SELECT * FROM tbl_user WHERE email = @Email AND password = @Password";

            var result = await _dbService.GetAll<User>(selectSql, parameters);
            return result?.FirstOrDefault();
        }

        public async Task<List<User>> Search(UserSearchDto search)
        {
            var selectSql = "SELECT * FROM tbl_user ";
            var whereSql = " WHERE 1=1 ";

            if (search.Id != null)
            {
                whereSql += " AND id = @Id";
            }
            if (!string.IsNullOrEmpty(search.Email))
            {
                whereSql += " AND email LIKE @Email";
            }
            if (!string.IsNullOrEmpty(search.PhoneNumber))
            {
                whereSql += " AND phone_number LIKE @PhoneNumber";
            }
            if (search.Role != null)
            {
                whereSql += " AND role = @Role";
            }
            if (search.IdLst != null && search.IdLst.Any())
            {
                whereSql += " AND id IN @IdLst";
            }

            whereSql += " ORDER BY create_time DESC";

            var userList = await _dbService.GetAll<User>(selectSql + whereSql, search);
            return userList;
        }

        public async Task<bool> Update(User user)
        {
            var updateSql = "UPDATE tbl_user SET ";

            if (!string.IsNullOrEmpty(user.Username))
            {
                updateSql += "username = @Username, ";
            }
            if (!string.IsNullOrEmpty(user.Password))
            {
                updateSql += "password = @Password, ";
            }
            if (user.Fullname != null)
            {
                updateSql += "fullname = @Fullname, ";
            }
            if (user.PhoneNumber != null)
            {
                updateSql += "phone_number = @PhoneNumber, ";
            }
            if (user.Address != null)
            {
                updateSql += "address = @Address, ";
            }
            if (user.Email != null)
            {
                updateSql += "email = @Email, ";
            }
            if (user.Role != 0)
            {
                updateSql += "role = @Role, ";
            }
            
            if (updateSql.EndsWith(", "))
            {
                updateSql = updateSql.Remove(updateSql.Length - 2);
            }

            updateSql += " WHERE id = @Id";

            var result = await _dbService.EditData(updateSql, user);
            return result > 0;
        }
    }
}
