using synced.Core.Results;
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
        Task<OperationResult<bool>> AddUserToProject(int projectId, int userId);
        Task<OperationResult<bool>> RemoveUserFromProject(int userId, int projectId);
        Task<OperationResult<List<UserDto>>> GetAllUsers(int projectId);
    }
}
