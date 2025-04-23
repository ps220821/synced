using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Interfaces
{
    public interface IUserProjectRepository
    {
        Task<List<ProjectUsers>> GetProjectUsers(int projectId);
        Task<int> AddUserToProject(ProjectUsers projectUser);
        Task<int> RemoveUserFromProject(int userId, int projectId);
    }
}
