using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DAL.Entities;
using synced_DALL.Interfaces;
using System.Data;
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

        public async Task<int> CreateAsync(Task task)
        {
            string query = @"
                INSERT INTO tasks (title, description, status, priority, deadline, user_id, project_id) 
                VALUES (@Title, @Description, @Status, @Priority, @Deadline, @User_Id, @Project_Id);
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Title", SqlDbType.NVarChar) { Value = task.Title },
                new SqlParameter("@Description", SqlDbType.NVarChar) { Value = task.Description },
                new SqlParameter("@Status", SqlDbType.Int) { Value = task.Status },
                new SqlParameter("@Priority", SqlDbType.Int) { Value = task.Priority },
                new SqlParameter("@Deadline", SqlDbType.DateTime) { Value = task.Deadline },
                new SqlParameter("@User_Id", SqlDbType.Int) { Value = (object?)task.User_id ?? DBNull.Value },
                new SqlParameter("@Project_Id", SqlDbType.Int) { Value = task.Project_id }
            };

            return  _dbHelper.ExecuteScalar(query, parameters);
        }


        public bool DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Task>> GetAllAsync(int projectId)
        {
            string query = "SELECT * FROM tasks WHERE project_id = @projectId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@projectId", SqlDbType.Int) { Value = projectId }
            };

            return  _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new Task
                {
                    Id = reader.GetInt32(reader.GetOrdinal("id")),  // Assuming 'project_id' is the task's Id
                    Title = reader.GetString(reader.GetOrdinal("title")),  // Assuming 'name' is the task's title
                    Description = reader.GetString(reader.GetOrdinal("description")),  // Assuming 'description' is the task's description
                    Status = Enum.TryParse(reader.GetString(reader.GetOrdinal("status")), true, out Status status) ? status : Status.todo,
                    Priority = Enum.TryParse(reader.GetString(reader.GetOrdinal("priority")), true, out Priorities priority) ? priority : Priorities.medium,
                    Deadline = reader.GetDateTime(reader.GetOrdinal("deadline")),  // Mapping 'start_date' to Deadline
                    User_id = reader.IsDBNull(reader.GetOrdinal("user_id")) ? 0 : reader.GetInt32(reader.GetOrdinal("user_id")),  // Default to 0 if 'user_id' is NULL
                    Project_id = reader.GetInt32(reader.GetOrdinal("project_id"))  // Mapping 'project_id' to Project_id
                };
            });
        }


        public synced_DAL.Entities.Task GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> UpdateAsync(Task task)
        {
            string query = @"
            UPDATE tasks SET 
            title = @Title,
            description = @Description,
            status = @Status,
            priority = @Priority,
            deadline = @Deadline,
            user_id = @User_Id,
            project_id = @Project_Id
            WHERE id = @Task_Id;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Title", SqlDbType.NVarChar) { Value = task.Title },
                new SqlParameter("@Description", SqlDbType.NVarChar) { Value = task.Description },
                new SqlParameter("@Status", SqlDbType.Int) { Value = task.Status },
                new SqlParameter("@Priority", SqlDbType.Int) { Value = task.Priority },
                new SqlParameter("@Deadline", SqlDbType.DateTime) { Value = task.Deadline },
                new SqlParameter("@User_Id", SqlDbType.Int) { Value = (object?)task.User_id ?? DBNull.Value },
                new SqlParameter("@Project_Id", SqlDbType.Int) { Value = task.Project_id },
                new SqlParameter("@Task_Id", SqlDbType.Int) { Value = task.Id }
            };

            return _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
