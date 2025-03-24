using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Interfaces
{
    public interface IUserProjectRepository
    {
        List<User> GetAllUsers(int projectId);

        bool AddUserToProject(Project_users projectUser);

        bool RemoveUserFromProject(int userId, int projectId);
    }
}
