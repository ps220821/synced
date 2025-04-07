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

namespace synced_DALL.Repositories
{
    public class UserProjectRepository : IUserProjectRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserProjectRepository(DatabaseHelper databaseHelper) {
            _dbHelper = databaseHelper;
        }

        public async Task<List<User>> GetAllUsers(int projectId)
        {
            string query = @"
                SELECT u.*
                FROM project_users pu
                INNER JOIN users u ON pu.user_id = u.id
                WHERE pu.project_id = @ProjectId";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId }
            };

            return  _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new User
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    username = reader.GetString(reader.GetOrdinal("username")),
                    firstname = reader.IsDBNull(reader.GetOrdinal("firstname")) ? null : reader.GetString(reader.GetOrdinal("firstname")),
                    lastname = reader.IsDBNull(reader.GetOrdinal("lastname")) ? null : reader.GetString(reader.GetOrdinal("lastname")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    password = reader.GetString(reader.GetOrdinal("password")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            });
        }

        public async Task<int> AddUserToProject(Project_users projectUser)
        {
            string query = @"
                INSERT INTO project_users (project_id, user_id, roles)
                VALUES (@ProjectId, @UserId, @Roles);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectUser.project_id },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = projectUser.user_id },
                new SqlParameter("@Roles", SqlDbType.Int) { Value = (int)projectUser.roles }
            };

            return (int)_dbHelper.ExecuteNonQuery(query, parameters);
        }

        public async Task<int> RemoveUserFromProject(int userId, int projectId)
        {
            string query = @"
                DELETE FROM project_users
                WHERE project_id = @ProjectId AND user_id = @UserId;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };

            return (int)_dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
