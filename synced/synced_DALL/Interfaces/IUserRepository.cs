using synced_DAL.Entities;
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
        bool Register(User user);  // Ensure return type is bool
        int Login(string email, string password);     // Use User object instead of string parameters OR adjust in UserRepository
    }
}

