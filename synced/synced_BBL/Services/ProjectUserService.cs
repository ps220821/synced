using synced_DALL.Interfaces;
using synced_DALL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Services
{
    public class ProjectUserService
    {
        private readonly IUserProjectRepository _userProjectRepository;

        public ProjectUserService(IProjectRepository projectRepository, IUserProjectRepository userProjectRepository)
        {
            _userProjectRepository = userProjectRepository;
        }

        public bool RemoveUserFromProject(int userId, int projectId)
       {
            return this._userProjectRepository.RemoveUserFromProject(userId,projectId);
       }
    }
}
