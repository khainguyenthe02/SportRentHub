﻿using SportRentHub.Entities.DTOs.User;
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
                "INSERT INTO tbl_user (username, password, fullname, phone_number, address, email, role, create_date, last_login, salt, token, status, bank_name, bank_number, bank_account) " +
                "VALUES (@Username, @Password, @Fullname, @PhoneNumber, @Address, @Email, @Role, @CreateDate, @LastLogin, @Salt, @Token, @Status, @BankName, @BankNumber, @BankAccount)",
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
                "SELECT * FROM tbl_user ORDER BY id DESC",
                new { }
            );
            return userList;
        }

        public async Task<User> GetByEmail(string email)
        {
            var user = await _dbService.GetAsync<User>(
                "SELECT * FROM tbl_user WHERE email = @Email",
                new { Email = email }
            );
            return user;
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
            if (search.IdLst != null && search.IdLst.Any())
            {
                whereSql += " and id IN @IdLst";
            }

            if (search.Id != null)
            {
                whereSql += " AND id = @Id";
            }
            if (!string.IsNullOrEmpty(search.Email))
            {
                whereSql += " AND email = @Email";
            }
            if (!string.IsNullOrEmpty(search.PhoneNumber))
            {
                whereSql += " AND phone_number = @PhoneNumber";
            }
            if (search.Role != null)
            {
                whereSql += " AND role = @Role";
            }
			if (search.Status != null)
			{
				whereSql += " AND status = @Status";
			}
			whereSql += " ORDER BY id DESC";

            var userList = await _dbService.GetAll<User>(selectSql + whereSql, search);
            return userList;
        }

        public async Task<bool> Update(User update)
        {
            var hasChanged = false;
            var updateSql = "UPDATE tbl_user SET ";

            if (!string.IsNullOrEmpty(update.Username))
            {
                updateSql += "username = @Username, ";
                hasChanged = true;
            }
            if (!string.IsNullOrEmpty(update.Password))
            {
                updateSql += "password = @Password, ";
                hasChanged = true;
            }
            if (update.Fullname != null)
            {
                updateSql += "fullname = @Fullname, ";
                hasChanged = true;
            }
            if (update.PhoneNumber != null)
            {
                updateSql += "phone_number = @PhoneNumber, ";
                hasChanged = true;
            }
            if (update.Address != null)
            {
                updateSql += "address = @Address, ";
                hasChanged = true;
            }
            if (update.Email != null)
            {
                updateSql += "email = @Email, ";
                hasChanged = true;
            }
            if (update.Role.HasValue)
            {
                updateSql += "role = @Role, ";
                hasChanged = true;
            }
            if (update.Token != null)
            {
                updateSql += "token = @Token, ";
                hasChanged = true;
            }
            if (update.Status.HasValue)
            {
				updateSql += "status = @Status, ";
				hasChanged = true;
			}
            if(update.BankAccount != null)
            {
                updateSql += "bank_account = @BankAccount, ";
                hasChanged = true;
            }
            if (update.BankName != null)
            {
                updateSql += "bank_name = @BankName, ";
                hasChanged = true;
            }
            if(update.BankNumber != null)
            {
                updateSql += "bank_number = @BankNumber, ";
                hasChanged = true;
            }
			if (!hasChanged)
            {
                return false;
            }

            if (updateSql.EndsWith(", "))
            {
                updateSql = updateSql.Remove(updateSql.Length - 2);
            }

            updateSql += " WHERE id = @Id";

            var result = await _dbService.EditData(updateSql, update);
            return result > 0;
        }
    }
}
