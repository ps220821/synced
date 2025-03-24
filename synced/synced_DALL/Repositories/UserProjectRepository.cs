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

        public List<User> GetAllUsers(int projectId)
        {
            string query = @" SELECT u.* FROM project_users pu INNER JOIN users u ON pu.user_id = u.id WHERE pu.project_id = @projectId";
            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@projectId", SqlDbType.Int) { Value = projectId }
            };

            return _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new User
                {
                    id = reader.GetInt32(reader.GetOrdinal("id")),
                    username = reader.GetString(reader.GetOrdinal("username")),
                    firstname = reader.GetString(reader.GetOrdinal("firstname")),
                    lastname = reader.GetString(reader.GetOrdinal("lastname")),
                    email = reader.GetString(reader.GetOrdinal("email")),
                    password = reader.GetString(reader.GetOrdinal("password")),
                    created_at = reader.GetDateTime(reader.GetOrdinal("created_at"))
                };
            });
        }

        public bool AddUserToProject(Project_users projectUser)
        {
            string query = "INSERT INTO project_Users (project_id, user_id, roles) VALUES (@ProjectId, @UserId, @Roles);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectUser.project_id },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = projectUser.user_id },
                new SqlParameter("@Roles", SqlDbType.NVarChar, 50) { Value =  (int)projectUser.roles }
            };

            return _dbHelper.ExecuteNonQuery(query, parameters);
        }

        public bool RemoveUserFromProject(int userId, int projectId)
        {
            string query = "DELETE FROM project_Users WHERE project_id = @ProjectId AND user_id = @UserId;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
            };

            return _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
