using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;

namespace synced_DALL.Repositories
{
    public class TaskCommentRepository : ItaskCommentRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public TaskCommentRepository(DatabaseHelper dbhelper)
        {
            _dbHelper = dbhelper;
        }

        public async Task<int> CreateAsync(TaskComment taskComment)
        {
            try
            {
                string query = @"
                    INSERT INTO task_comments (user_id, task_id, comment, created_at)
                    VALUES (@User_Id, @Task_Id, @Comment, @Created_At);
                    SELECT SCOPE_IDENTITY();";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@User_Id", SqlDbType.Int)    { Value = taskComment.UserId },
                    new SqlParameter("@Task_Id", SqlDbType.Int)    { Value = taskComment.TaskId },
                    new SqlParameter("@Comment", SqlDbType.NVarChar) { Value = taskComment.Comment },
                    new SqlParameter("@Created_At", SqlDbType.DateTime) { Value = taskComment.CreatedAt }
                };

                var result = await _dbHelper.ExecuteScalar<int>(query, parameters);
                return Convert.ToInt32(result);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error creating a task comment.", ex);
            }
        }

        public async Task<List<TaskComment>> GetAllAsync(int taskId)
        {
            try
            {
                string query = @"
                    SELECT 
                        tc.id               AS Id,
                        tc.user_id          AS UserId,
                        tc.task_id          AS TaskId,
                        tc.comment          AS Comment,
                        tc.created_at       AS CreatedAt,

                        u.id                AS OwnerUserId,
                        u.username          AS OwnerUsername,
                        u.firstname         AS OwnerFirstname,
                        u.lastname          AS OwnerLastname,
                        u.email             AS OwnerEmail,
                        u.password          AS OwnerPassword,
                        u.created_at        AS OwnerCreatedAt,

                        t.id                AS TId,
                        t.title             AS Title,
                        t.description       AS Description,
                        t.status            AS Status,
                        t.priority          AS Priority,
                        t.deadline          AS Deadline,
                        t.user_id           AS TaskUserId,
                        t.project_id        AS ProjectId
                    FROM task_comments tc
                    JOIN users u ON tc.user_id = u.id
                    JOIN tasks t ON tc.task_id = t.id
                    WHERE tc.task_id = @TaskId;";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@TaskId", SqlDbType.Int) { Value = taskId }
                };

                return await _dbHelper.ExecuteReader(query, parameters, reader =>
                {
                    var ownerUser = User.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("OwnerUserId")),
                        reader.GetString(reader.GetOrdinal("OwnerUsername")),
                        reader.GetString(reader.GetOrdinal("OwnerFirstname")),
                        reader.GetString(reader.GetOrdinal("OwnerLastname")),
                        reader.GetString(reader.GetOrdinal("OwnerEmail")),
                        reader.GetString(reader.GetOrdinal("OwnerPassword")),
                        reader.GetDateTime(reader.GetOrdinal("OwnerCreatedAt"))
                    );

                    var task = Entities.Task.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("TId")),
                        reader.GetString(reader.GetOrdinal("Title")),
                        reader.GetString(reader.GetOrdinal("Description")),
                        Enum.Parse<Status>(reader.GetString(reader.GetOrdinal("Status")), true),
                        Enum.Parse<Priorities>(reader.GetString(reader.GetOrdinal("Priority")), true),
                        reader.GetDateTime(reader.GetOrdinal("Deadline")),
                        reader.GetInt32(reader.GetOrdinal("TaskUserId")),
                        reader.GetInt32(reader.GetOrdinal("ProjectId")),
                        null,
                        null
                    );

                    return TaskComment.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("Id")),
                        reader.GetInt32(reader.GetOrdinal("UserId")),
                        reader.GetInt32(reader.GetOrdinal("TaskId")),
                        reader.GetString(reader.GetOrdinal("Comment")),
                        reader.GetDateTime(reader.GetOrdinal("CreatedAt")),
                        ownerUser,
                        task
                    );
                });
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving task comments.", ex);
            }
        }
    }
}
