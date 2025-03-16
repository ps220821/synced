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
