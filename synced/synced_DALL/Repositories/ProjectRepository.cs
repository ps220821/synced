using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DAL.Interfaces;
using synced_DALL.Entities;
using synced_DALL.Interfaces;

namespace synced_DALL.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public ProjectRepository(DatabaseHelper dbhelper)
        {
            _dbHelper = dbhelper;
        }

        public async Task<List<Project>> GetAllAsync(int userId)
        {
            try
            {
                string query = @"
            SELECT 
                p.id           AS ProjectId,
                p.name         AS ProjectName,
                p.description  AS ProjectDescription,
                p.start_date   AS StartDate,
                p.end_date     AS EndDate,
                p.owner        AS OwnerId,

                u.id           AS OwnerUserId,
                u.username     AS OwnerUsername,
                u.firstname    AS OwnerFirstname,
                u.lastname     AS OwnerLastname,
                u.email        AS OwnerEmail,
                u.password     AS OwnerPassword,
                u.created_at   AS OwnerCreatedAt
            FROM project_users pu
            JOIN projects p ON pu.project_id = p.id
            LEFT JOIN users u ON p.owner = u.id
            WHERE pu.user_id = @UserId;";

                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@UserId", SqlDbType.Int) { Value = userId }
        };

                return await _dbHelper.ExecuteReader(query, parameters, reader =>
                {
                    User ownerUser = User.Rehydrate(
                       reader.GetInt32(reader.GetOrdinal("OwnerUserId")),  
                       reader.GetString(reader.GetOrdinal("OwnerUsername")),
                       reader.GetString(reader.GetOrdinal("OwnerFirstname")), 
                       reader.GetString(reader.GetOrdinal("OwnerLastname")),  
                       reader.GetString(reader.GetOrdinal("OwnerEmail")),   
                       reader.GetString(reader.GetOrdinal("OwnerPassword")),
                       reader.GetDateTime(reader.GetOrdinal("OwnerCreatedAt"))
   );

                    Project project = Project.Rehydrate(
                        reader.GetInt32(reader.GetOrdinal("ProjectId")),
                        reader.GetString(reader.GetOrdinal("ProjectName")),
                        reader.GetString(reader.GetOrdinal("ProjectDescription")),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                        DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("EndDate"))),
                        reader.GetInt32(reader.GetOrdinal("OwnerId")),
                        ownerUser
                    );

                    return project;
                });
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving projects.", ex);
            }
        }

        public async Task<int> CreateAsync(Project project)
        {
            try
            {
                string query = @"
                    INSERT INTO projects (name, description, start_date, end_date, owner)
                    VALUES (@Name, @Description, @StartDate, @EndDate, @Owner);
                    SELECT SCOPE_IDENTITY();";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Name", SqlDbType.NVarChar) { Value = project.Name },
                    new SqlParameter("@Description", SqlDbType.NVarChar)
                    {
                        Value = project.Description ?? (object)DBNull.Value
                    },
                    new SqlParameter("@StartDate", SqlDbType.Date) { Value = project.Start_Date },
                    new SqlParameter("@EndDate", SqlDbType.Date) { Value = project.End_Date },
                    new SqlParameter("@Owner", SqlDbType.Int) { Value = project.Owner }
                };

                return await _dbHelper.ExecuteScalar<int>(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error creating project.", ex);
            }
        }

        public async Task<int> DeleteAsync(int projectId)
        {
            try
            {
                string query = @"DELETE FROM projects WHERE id = @ProjectId;";
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ProjectId", SqlDbType.Int) { Value = projectId }
                };

                return await _dbHelper.ExecuteNonQuery(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error deleting project.", ex);
            }
        }
    }
}
