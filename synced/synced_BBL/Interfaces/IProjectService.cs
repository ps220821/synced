using synced.Core.Results;
using synced_BBL.Dtos;
using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Interfaces
{
    public interface IProjectService
    {
        Task<OperationResult<List<ProjectDto>>> GetAllProjects(int id);

        Task<OperationResult<bool>> CreateProject(ProjectDto project);

        Task<OperationResult<bool>> DeleteProject(int id);
    }
}
