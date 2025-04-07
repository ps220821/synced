using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DAL.Entities;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_DALL.Repositories
{
    public class TaskCommentRepository : ItaskCommentRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public TaskCommentRepository(DatabaseHelper dbhelper)
        {
            _dbHelper = dbhelper; // Initialize the DatabaseHelper instance
        }

        public int CreateAsync(TaskComment taskComment) // Geen async/Task
        {
            string query = @"
                INSERT INTO task_comments (user_id, task_id, comment, created_at)
                VALUES (@User_Id, @Task_Id, @Comment, @Created_At);
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@User_Id", SqlDbType.Int) { Value = taskComment.user_id },
                new SqlParameter("@Task_Id", SqlDbType.Int) { Value = taskComment.task_id },
                new SqlParameter("@Comment", SqlDbType.NVarChar) { Value = taskComment.comment ?? (object)DBNull.Value },
                new SqlParameter("@Created_At", SqlDbType.DateTime) { Value = taskComment.created_at }
            };

            var result = _dbHelper.ExecuteScalar(query, parameters);
            return Convert.ToInt32(result);
        }

        public List<TaskCommentExtended> GetAllAsync(int taskId) // Geen async/Task
        {
            string query = @"
                SELECT tc.*, u.username
                FROM task_comments tc
                JOIN users u ON tc.user_id = u.id
                WHERE tc.task_id = @Task_Id;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Task_Id", SqlDbType.Int) { Value = taskId }
            };

            return _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new TaskCommentExtended
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    user_id = reader.GetInt32(reader.GetOrdinal("user_id")),
                    task_id = reader.GetInt32(reader.GetOrdinal("task_id")),
                    comment = reader.IsDBNull(reader.GetOrdinal("comment")) ? null : reader.GetString(reader.GetOrdinal("comment")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at")),
                    username = reader.GetString(reader.GetOrdinal("username"))
                };
            });
        }
    }
}
