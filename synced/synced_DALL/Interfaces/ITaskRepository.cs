using synced_DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = synced_DAL.Entities.Task;

namespace synced_DALL.Interfaces
{
    public interface ITaskRepository
    {
        List<Task> GetAllAsync(int id);

        Task GetByIdAsync(int id);

        int CreateAsync(Task task);

        bool UpdateAsync(Task task);

        bool DeleteAsync(int id);
    }
}

