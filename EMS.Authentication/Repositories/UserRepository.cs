using EMS.Authentication.Repositories.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TF.Authentication.Entities;

namespace TF.Authentication.Repositories
{
    public interface IUserRepository
    {
        Task<User> Get(UserFilter userFilter);
        Task<bool> ChangePassword(User user);
    }
    public class UserRepository : IUserRepository
    {
        private readonly EMSContext context;
        public UserRepository(EMSContext _context)
        {
            context = _context;
        }

        private IQueryable<UserDAO> DynamicFilter(IQueryable<UserDAO> query, UserFilter userFilter)
        {
            if (!string.IsNullOrEmpty(userFilter.Username))
            {
                query = query.Where(u => u.Username.Equals(userFilter.Username));
            }
            if (userFilter.IsAdmin.HasValue)
            {
                query = query.Where(u => u.IsAdmin.Equals(userFilter.IsAdmin.Value));
            }
            return query;
        }

        public async Task<User> Get(UserFilter userFilter)
        {
            IQueryable<UserDAO> users = context.User;
            UserDAO userDAO = await DynamicFilter(users, userFilter).FirstOrDefaultAsync();
            User user = null;
            if (userDAO == null) return user;
            else
            {
                user = new User()
                {
                    Id = userDAO.Id,
                    Username = userDAO.Username,
                    Password = userDAO.Password,
                    Salt = userDAO.Salt,
                    IsAdmin = userDAO.IsAdmin,
                    StudentId = userDAO.StudentId,
                    Email = userDAO.Email
                };
            }

            return user;
        }

        public async Task<bool> ChangePassword(User user)
        {
            await context.User.Where(u => u.Id.Equals(user.Id)).UpdateFromQueryAsync(u => new UserDAO
            {
                Password = user.Password,
                Salt = user.Salt
            });

            return true;
        }
    }
}
