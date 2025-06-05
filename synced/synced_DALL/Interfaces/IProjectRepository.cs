using synced_DALL.Entities;
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
        Task<List<Project>> GetAllAsync(int id);
        Task<int> CreateAsync(Project project);
        Task<int> DeleteAsync(int id);
    }
}
