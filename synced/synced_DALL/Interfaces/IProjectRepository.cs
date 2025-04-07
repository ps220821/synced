using synced_DAL.Entities;
using synced_DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Interfaces
{
    public interface IProjectRepository 
    {
        List<Project> GetAllAsync(int id);

        Task<Project> GetByIdAsync(int id);

        int CreateAsync(Project entity);

        bool UpdateAsync(Project entity);

        int DeleteAsync(int id);
    }
}
