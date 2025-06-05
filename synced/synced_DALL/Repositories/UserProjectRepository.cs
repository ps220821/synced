using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;

namespace synced_DALL.Repositories
{
    public class UserProjectRepository : IUserProjectRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserProjectRepository(DatabaseHelper databaseHelper)
        {
            _dbHelper = databaseHelper;
        }

        public async Task<List<ProjectUsers>> GetProjectUsers(int projectId)
        {
            try
            {
                string query = @"
                    SELECT 
                        pu.id                AS Id,
                        pu.project_id        AS ProjectId,
                        pu.user_id           AS UserId,
                        pu.roles             AS Role,

                        u.id                 AS OwnerId,
                        u.username           AS OwnerUsername,
                        u.firstname          AS OwnerFirstname,
                        u.lastname           AS OwnerLastname,
                        u.email              AS OwnerEmail,
                        u.password           AS OwnerPassword,
                        u.created_at         AS OwnerCreatedAt,

                        p.id                 AS ProjectId2,
                        p.name               AS ProjectName,
                        p.description        AS ProjectDescription,
                        p.start_date         AS StartDate,
                        p.end_date           AS EndDate,
                        p.owner              AS ProjectOwnerId
                    FROM project_users pu
                    INNER JOIN users u ON pu.user_id    = u.id
                    INNER JOIN projects p ON pu.project_id = p.id
                    WHERE pu.project_id = @ProjectId;";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId }
                };

                return await _dbHelper.ExecuteReader(query, parameters, reader =>
                {
                    // Stap 1: kolomwaarden inlezen
                    int puId = reader.GetInt32(reader.GetOrdinal("Id"));
                    int projIdFromPU = reader.GetInt32(reader.GetOrdinal("ProjectId"));
                    int uId = reader.GetInt32(reader.GetOrdinal("UserId"));
                    Roles role = (Roles)reader.GetInt32(reader.GetOrdinal("Role"));

                    // Stap 2: User rehydratatie
                    var ownerUser = User.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("OwnerId")),
                        reader.GetString(reader.GetOrdinal("OwnerUsername")),
                        reader.GetString(reader.GetOrdinal("OwnerFirstname")),
                        reader.GetString(reader.GetOrdinal("OwnerLastname")),
                        reader.GetString(reader.GetOrdinal("OwnerEmail")),
                        reader.GetString(reader.GetOrdinal("OwnerPassword")),
                        reader.GetDateTime(reader.GetOrdinal("OwnerCreatedAt"))
                    );

                    // Stap 3: Project rehydratatie
                    var project = Project.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("ProjectId2")),
                        reader.GetString(reader.GetOrdinal("ProjectName")),
                        reader.GetString(reader.GetOrdinal("ProjectDescription")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("EndDate"))),
                        reader.GetInt32(reader.GetOrdinal("ProjectOwnerId")),
                        ownerUser
                    );

                    // Stap 4: ProjectUsers rehydratatie
                    return ProjectUsers.Rehydrate(
                        puId,
                        projIdFromPU,
                        uId,
                        role,
                        project,
                        ownerUser
                    );
                });
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving project users.", ex);
            }
        }

        public async Task<int> AddUserToProject(ProjectUsers projectUser)
        {
            try
            {
                string query = @"
                    INSERT INTO project_users (project_id, user_id, roles)
                    VALUES (@ProjectId, @UserId, @Roles);";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectUser.ProjectId },
                    new SqlParameter("@UserId",    SqlDbType.Int) { Value = projectUser.UserId    },
                    new SqlParameter("@Roles",     SqlDbType.Int) { Value = (int)projectUser.Role  }
                };

                return await _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error adding user to project.", ex);
            }
        }

        public async Task<int> RemoveUserFromProject(int userId, int projectId)
        {
            try
            {
                string query = @"
                    DELETE FROM project_users
                    WHERE project_id = @ProjectId AND user_id = @UserId;";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId },
                    new SqlParameter("@UserId",    SqlDbType.Int) { Value = userId   }
                };

                return await _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error removing user from project.", ex);
            }
        }
    }
}
