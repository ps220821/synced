using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task = synced_DALL.Entities.Task;

namespace synced_DALL.Repositories
{
    public class TaskCommentRepository : ItaskCommentRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public TaskCommentRepository(DatabaseHelper dbhelper)
        {
            _dbHelper = dbhelper; // Initialize the DatabaseHelper instance
        }

        public async Task<int> CreateAsync(TaskComment taskComment) // Geen async/Task
        {
            string query = @"
                INSERT INTO task_comments (user_id, task_id, comment, created_at)
                VALUES (@User_Id, @Task_Id, @Comment, @Created_At);
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@User_Id", SqlDbType.Int) { Value = taskComment.UserId },
                new SqlParameter("@Task_Id", SqlDbType.Int) { Value = taskComment.TaskId },
                new SqlParameter("@Comment", SqlDbType.NVarChar) { Value = taskComment.Comment ?? (object)DBNull.Value },
                new SqlParameter("@Created_At", SqlDbType.DateTime) { Value = taskComment.CreatedAt }
            };

            var result = await _dbHelper.ExecuteScalar<int>(query, parameters);
            return Convert.ToInt32(result);
        }

        public async Task<List<TaskComment>> GetAllAsync(int taskId)
        {
            string query = @"
                SELECT 
                    tc.id AS Id,
                    tc.user_id AS UserId,
                    tc.task_id AS TaskId,
                    tc.comment AS Comment,
                    tc.created_at AS CreatedAt,

                    u.id AS UId,
                    u.username AS Username,
                    u.firstname AS Firstname,
                    u.lastname AS Lastname,
                    u.email AS Email,
                    u.password AS Password,
                    u.created_at AS UserCreatedAt,

                    t.id AS TId,
                    t.title AS Title,
                    t.description AS Description,
                    t.status AS Status,
                    t.priority AS Priority,
                    t.deadline AS Deadline,
                    t.user_id AS TaskUserId,
                    t.project_id AS ProjectId
                FROM task_comments tc
                JOIN users u ON tc.user_id = u.id
                JOIN tasks t ON tc.task_id = t.id
                WHERE tc.task_id = @TaskId;";

            var parameters = new List<SqlParameter> { new SqlParameter("@TaskId", SqlDbType.Int) { Value = taskId } };

            return await _dbHelper.ExecuteReader(query, parameters, reader => new TaskComment
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                TaskId = reader.GetInt32(reader.GetOrdinal("TaskId")),
                Comment = reader.GetString(reader.GetOrdinal("Comment")),
                CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                User = new User
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UId")),
                    Username = reader.GetString(reader.GetOrdinal("Username")),
                    Firstname = reader.GetString(reader.GetOrdinal("Firstname")),
                    Lastname = reader.GetString(reader.GetOrdinal("Lastname")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    Password = reader.GetString(reader.GetOrdinal("Password")),
                    CreatedAt = reader.GetDateTime(reader.GetOrdinal("UserCreatedAt"))
                },
                Task = new Task
                {
                    Id = reader.GetInt32(reader.GetOrdinal("TId")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Status = Enum.Parse<Status>(reader.GetString(reader.GetOrdinal("Status")), true),
                    Priority = Enum.Parse<Priorities>(reader.GetString(reader.GetOrdinal("Priority")), true),
                    Deadline = reader.GetDateTime(reader.GetOrdinal("Deadline")),
                    UserId = reader.IsDBNull(reader.GetOrdinal("TaskUserId")) ? null : reader.GetInt32(reader.GetOrdinal("TaskUserId")),
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId"))
                }
            });
        }

    }
}
