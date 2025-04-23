using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using System.Data;
using Task = synced_DALL.Entities.Task;


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
                new SqlParameter("@User_Id", SqlDbType.Int) { Value = (object?)task.UserId ?? DBNull.Value },
                new SqlParameter("@Project_Id", SqlDbType.Int) { Value = task.ProjectId }
            };

            return await _dbHelper.ExecuteScalar<int>(query, parameters);
        }


        public bool DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Task>> GetAllAsync(int projectId)
        {
            string query = @"
        SELECT 
            t.id AS TaskId, 
            t.title AS Title, 
            t.description AS Description, 
            t.status AS Status, 
            t.priority AS Priority, 
            t.deadline AS Deadline, 
            t.user_id AS UserId, 
            t.project_id AS ProjectId,
            u.id AS UserId, 
            u.username AS Username, 
            u.firstname AS Firstname, 
            u.lastname AS Lastname, 
            u.email AS Email, 
            u.password AS Password, 
            u.created_at AS CreatedAt,
            p.id AS ProjectId, 
            p.name AS ProjectName, 
            p.description AS ProjectDescription, 
            p.start_date AS ProjectStartDate, 
            p.end_date AS ProjectEndDate, 
            p.owner AS ProjectOwner
        FROM tasks t
        LEFT JOIN users u ON t.user_id = u.id
        LEFT JOIN projects p ON t.project_id = p.id
        WHERE t.project_id = @ProjectId;";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId }
    };

            return await _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                var task = new Task
                {
                    Id = reader.GetInt32(reader.GetOrdinal("TaskId")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Status = Enum.TryParse(reader.GetString(reader.GetOrdinal("Status")), true, out Status status) ? status : Status.todo,
                    Priority = Enum.TryParse(reader.GetString(reader.GetOrdinal("Priority")), true, out Priorities priority) ? priority : Priorities.medium,
                    Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline")),
                    UserId = reader.IsDBNull(reader.GetOrdinal("UserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("UserId")),
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId"))
                };

                // Koppel de User object
                if (!reader.IsDBNull(reader.GetOrdinal("UserId")))
                {
                    task.User = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("UserId")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Firstname = reader.IsDBNull(reader.GetOrdinal("Firstname")) ? null : reader.GetString(reader.GetOrdinal("Firstname")),
                        Lastname = reader.IsDBNull(reader.GetOrdinal("Lastname")) ? null : reader.GetString(reader.GetOrdinal("Lastname")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    };
                }

                // Koppel de Project object
                task.Project = new Project
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    Name = reader.GetString(reader.GetOrdinal("ProjectName")),
                    Description = reader.GetString(reader.GetOrdinal("ProjectDescription")),
                    Start_Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ProjectStartDate"))),
                    End_Date = reader.IsDBNull(reader.GetOrdinal("ProjectEndDate"))
                                ? default
                                : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ProjectEndDate"))),
                    Owner = reader.GetInt32(reader.GetOrdinal("ProjectOwner"))
                };

                return task;
            });
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
                new SqlParameter("@User_Id", SqlDbType.Int) { Value = (object?)task.UserId ?? DBNull.Value },
                new SqlParameter("@Project_Id", SqlDbType.Int) { Value = task.ProjectId },
                new SqlParameter("@Task_Id", SqlDbType.Int) { Value = task.Id }
            };

            return await _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
