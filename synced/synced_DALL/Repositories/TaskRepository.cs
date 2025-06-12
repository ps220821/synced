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
            _dbHelper = dbhelper;
        }

        public async Task<int> CreateAsync(Task task)
        {
            try
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
            catch (SqlException ex)
            {
                throw new DatabaseException("Error creating a task.", ex);
            }
        }

        public bool DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Task>> GetAllAsync(int projectId)
        {
            try
            {
                string query = @"
                                   SELECT
                  t.id            AS TaskId,
                  t.title         AS TaskTitle,
                  t.description   AS TaskDescription,
                  t.status        AS TaskStatus,
                  t.priority      AS TaskPriority,
                  t.deadline      AS TaskDeadline,
                  t.user_id       AS TaskUserId,
                  t.project_id    AS TaskProjectId,

                  u.id            AS OwnerUserId,
                  u.username      AS OwnerUsername,
                  u.firstname     AS OwnerFirstname,
                  u.lastname      AS OwnerLastname,
                  u.email         AS OwnerEmail,
                  u.password      AS OwnerPassword,
                  u.created_at    AS OwnerCreatedAt,

                  p.id            AS ProjectId,
                  p.name          AS ProjectName,
                  p.description   AS ProjectDescription,
                  p.start_date    AS ProjectStartDate,
                  p.end_date      AS ProjectEndDate,
                  p.owner         AS ProjectOwnerId
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
                
                
                    int? userId = reader.IsDBNull(reader.GetOrdinal("TaskUserId"))
                                         ? (int?)null
                                         : reader.GetInt32(reader.GetOrdinal("TaskUserId"));
                    int projectIdFromTask = reader.GetInt32(reader.GetOrdinal("TaskProjectId"));

                    User ownerUser = null;
                    if (userId.HasValue)
                    {
                        ownerUser = User.Rehydrate(
                            reader.GetInt32(reader.GetOrdinal("OwnerUserId")),
                            reader.GetString(reader.GetOrdinal("OwnerUsername")),
                            reader.GetString(reader.GetOrdinal("OwnerFirstname")),
                            reader.GetString(reader.GetOrdinal("OwnerLastname")),
                            reader.GetString(reader.GetOrdinal("OwnerEmail")),
                            reader.GetString(reader.GetOrdinal("OwnerPassword")),
                            reader.GetDateTime(reader.GetOrdinal("OwnerCreatedAt"))
                        );
                    }

                    Project project = Project.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("ProjectId")),
                        reader.GetString(reader.GetOrdinal("ProjectName")),
                        reader.GetString(reader.GetOrdinal("ProjectDescription")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ProjectStartDate"))),
                        reader.IsDBNull(reader.GetOrdinal("ProjectEndDate"))
                            ? DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ProjectStartDate")))
                            : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("ProjectEndDate"))),
                        reader.GetInt32(reader.GetOrdinal("ProjectOwnerId")),
                        ownerUser
                    );

                    // ─────────── Stap 4: Task rehydraten ───────────
                    return Task.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("TaskId")),
                        reader.GetString(reader.GetOrdinal("TaskTitle")),
                        reader.GetString(reader.GetOrdinal("TaskDescription")),
                        Enum.TryParse( reader.GetString(reader.GetOrdinal("TaskStatus")), true, out Status sVal ) ? sVal : Status.todo,
                        Enum.TryParse( reader.GetString(reader.GetOrdinal("TaskPriority")), true, out Priorities pVal ) ? pVal : Priorities.medium,
                        reader.GetDateTime(reader.GetOrdinal("TaskDeadline")),
                        reader.IsDBNull(reader.GetOrdinal("TaskUserId")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("TaskUserId")),
                        reader.GetInt32(reader.GetOrdinal("TaskProjectId")),
                        ownerUser,
                        project
                    );
                });
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving tasks for this project.", ex);
            }
        }

        public async Task<int> UpdateAsync(Task task)
        {
            try
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
            catch (SqlException ex)
            {
                throw new DatabaseException("Error updating task.", ex);
            }
        }
    }
}
