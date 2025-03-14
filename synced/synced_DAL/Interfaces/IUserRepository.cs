using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DAL.Interfaces
{
    public interface IUserRepository
    {
        bool Register(User user);

        bool Login(User user);
    }
}
