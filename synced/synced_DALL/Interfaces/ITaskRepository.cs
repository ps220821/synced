using synced_DALL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = synced_DALL.Entities.Task;

namespace synced_DALL.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<Task>> GetAllAsync(int id);


        Task<int> CreateAsync(Task task);

        Task<int> UpdateAsync(Task task);

        Task<bool> DeleteAsync(int id);
    }
}

