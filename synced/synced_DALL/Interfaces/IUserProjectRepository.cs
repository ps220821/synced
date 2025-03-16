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
        bool AddUserToProject(Project_users projectUser);

        bool RemoveUserFromProject(int userId, int projectId);
    }
}
