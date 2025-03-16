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
        List<ProjectDto> GetAllProjects(int userId);

        bool CreateProject(ProjectDto project);

        bool DeleteProject(int id);
    }
}
