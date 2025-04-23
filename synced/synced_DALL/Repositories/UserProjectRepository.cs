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

namespace synced_DALL.Repositories
{
    public class UserProjectRepository : IUserProjectRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserProjectRepository(DatabaseHelper databaseHelper) {
            _dbHelper = databaseHelper;
        }

        public async Task<List<ProjectUsers>> GetProjectUsers(int projectId)
        {
            string query = @"
        SELECT
  pu.user_id        AS UserId,
  pu.roles           AS Role,
  u.id              AS Id,
  u.username        AS Username,
  u.firstname       AS Firstname,
  u.lastname        AS Lastname,
  u.email           AS Email,
  u.password        AS Password,
  u.created_at      AS CreatedAt,
  p.id              AS ProjectId,
  p.name            AS ProjectName,
  p.description     AS ProjectDescription,
  p.start_date      AS StartDate,
  p.end_date        AS EndDate,
  p.owner           AS Owner
FROM project_users pu
INNER JOIN users   u ON pu.user_id    = u.id
INNER JOIN projects p ON pu.project_id = p.id
WHERE pu.project_id = @ProjectId
";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId }
    };

            return await _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new ProjectUsers
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    ProjectId = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                    Role = (Roles)reader.GetInt32(reader.GetOrdinal("Role")),

                    User = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        Username = reader.GetString(reader.GetOrdinal("Username")),
                        Firstname = reader.IsDBNull(reader.GetOrdinal("Firstname"))
                                       ? null
                                       : reader.GetString(reader.GetOrdinal("Firstname")),
                        Lastname = reader.IsDBNull(reader.GetOrdinal("Lastname"))
                                       ? null
                                       : reader.GetString(reader.GetOrdinal("Lastname")),
                        Email = reader.GetString(reader.GetOrdinal("Email")),
                        Password = reader.GetString(reader.GetOrdinal("Password")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt"))
                    },

                    Project = new Project
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                        Name = reader.GetString(reader.GetOrdinal("ProjectName")),
                        Description = reader.GetString(reader.GetOrdinal("ProjectDescription")),
                        Start_Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                        End_Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("EndDate"))),
                        Owner = reader.GetInt32(reader.GetOrdinal("Owner"))
                    }
                };
            });
        }



        public async Task<int> AddUserToProject(ProjectUsers projectUser)
        {
            string query = @"
                INSERT INTO project_users (project_id, user_id, roles)
                VALUES (@ProjectId, @UserId, @Roles);";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectUser.ProjectId },
                new SqlParameter("@UserId", SqlDbType.Int) { Value = projectUser.UserId },
                new SqlParameter("@Roles", SqlDbType.Int) { Value = (int)projectUser.Role }
            };

            return await _dbHelper.ExecuteNonQuery(query, parameters);
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

            return await _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
