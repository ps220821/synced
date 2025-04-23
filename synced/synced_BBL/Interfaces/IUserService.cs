using synced.Core.Results;
using synced_BBL.Dtos;
using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Interfaces
{
    public interface IUserService
    {
        Task<int> GetUserByEmail(string email);
        Task<OperationResult<int>> RegisterUser(UserDto userDto);
        Task<OperationResult<int>> LoginUser(LoginDto login);
    }
}
