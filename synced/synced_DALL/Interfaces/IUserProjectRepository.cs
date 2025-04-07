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
        Task<List<User>> GetAllUsers(int projectId);
        Task<int> AddUserToProject(Project_users projectUser);
        Task<int> RemoveUserFromProject(int userId, int projectId);
    }
}
