using synced_BBL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Interfaces
{
    public interface IProjectUserService
    {
        bool AddUserToProject(int projectId, int userId);
        bool RemoveUserFromProject(int userId, int projectId);
        List<UserDto> GetAllUsers(int projectId);
    }
}
