using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DAL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = synced_DAL.Entities.Task;


namespace synced_DALL.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public TaskRepository(DatabaseHelper dbhelper)
        {
            _dbHelper = dbhelper; // Initialize the DatabaseHelper instance
        }
        public int CreateAsync(Task task)
        {
            throw new NotImplementedException();
        }

        public bool DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public List<Task> GetAllAsync(int id)
        {
            string query = "SELECT * FROM dbo.tasks WHERE project_id = @projectId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@projectId", SqlDbType.Int) { Value = id }
            };

            return _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new Task
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),  // Assuming 'project_id' is the task's Id
                    Title = reader.GetString(reader.GetOrdinal("title")),  // Assuming 'name' is the task's title
                    Description = reader.GetString(reader.GetOrdinal("description")),  // Assuming 'description' is the task's description
                    Status = Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out Status status) ? status : Status.todo,
                    Priority = Enum.TryParse(reader.GetString(reader.GetOrdinal("priority")), true, out Priorities priority) ? priority : Priorities.medium,
                    Deadline = reader.GetDateTime(reader.GetOrdinal("deadline")),  // Mapping 'start_date' to Deadline
                    User_id = reader.GetInt32(reader.GetOrdinal("user_id")),  // Mapping 'user_id' to User_id
                    Project_id = reader.GetInt32(reader.GetOrdinal("project_id"))  // Mapping 'project_id' to Project_id
                };
            });
        }

        public synced_DAL.Entities.Task GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateAsync(synced_DAL.Entities.Task task)
        {
            throw new NotImplementedException();
        }
    }
}
