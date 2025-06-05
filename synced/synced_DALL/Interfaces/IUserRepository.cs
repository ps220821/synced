using synced.Core.Results;
using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByMail(string email);
        Task<int> Register(User user);
        Task<int> CheckEmailExists(string email);
    }
}

