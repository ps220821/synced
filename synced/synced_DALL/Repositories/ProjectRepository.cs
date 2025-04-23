using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DALL.Entities;
using synced_DALL.Interfaces;
using System.Data;

namespace synced_DALL.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public ProjectRepository(DatabaseHelper dbhelper)
        {
            _dbHelper = dbhelper; 
        }
        // get All projects
        public async Task<List<Project>> GetAllAsync(int id)
        {
            string query = @"
        SELECT 
            p.id AS ProjectId, 
            pu.user_id AS UserId, 
            p.owner AS Owner, 
            p.name AS Name, 
            p.description AS Description, 
            p.start_date AS StartDate, 
            p.end_date AS EndDate,
            u.id AS OwnerId,
            u.username AS OwnerUsername,
            u.firstname AS OwnerFirstname,
            u.lastname AS OwnerLastname,
            u.email AS OwnerEmail,
            u.password AS OwnerPassword,
            u.created_at AS OwnerCreatedAt
        FROM project_users pu
        JOIN projects p ON pu.project_id = p.id
        LEFT JOIN users u ON p.owner = u.id
        WHERE pu.user_id = @id;";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@id", SqlDbType.Int) { Value = id }
    };

            return await _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                var project = new Project
                {
                    Id = reader.GetInt32(reader.GetOrdinal("ProjectId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Description = reader.GetString(reader.GetOrdinal("Description")),
                    Start_Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                    End_Date = reader.IsDBNull(reader.GetOrdinal("EndDate"))
                                ? default
                                : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("EndDate"))),
                    Owner = reader.GetInt32(reader.GetOrdinal("Owner")),
                    User = new User
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                        Username = reader.GetString(reader.GetOrdinal("OwnerUsername")),
                        Firstname = reader.IsDBNull(reader.GetOrdinal("OwnerFirstname")) ? null : reader.GetString(reader.GetOrdinal("OwnerFirstname")),
                        Lastname = reader.IsDBNull(reader.GetOrdinal("OwnerLastname")) ? null : reader.GetString(reader.GetOrdinal("OwnerLastname")),
                        Email = reader.GetString(reader.GetOrdinal("OwnerEmail")),
                        Password = reader.GetString(reader.GetOrdinal("OwnerPassword")),
                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("OwnerCreatedAt"))
                    }
                };

                return project;
            });
        }


        public Task<Project> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateAsync(Project project)
        {
            string query = @"
                INSERT INTO projects (name, description, start_date, end_date, owner)
                VALUES (@Name, @Description, @Start_Date, @End_Date, @Owner);
                SELECT SCOPE_IDENTITY();";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Name", SqlDbType.NVarChar) { Value = project.Name },
                new SqlParameter("@Description", SqlDbType.NVarChar) { Value = project.Description },
                new SqlParameter("@Start_Date", SqlDbType.Date) { Value = project.Start_Date },
                new SqlParameter("@End_Date", SqlDbType.Date) { Value = project.End_Date },
                new SqlParameter("@Owner", SqlDbType.Int) { Value = project.Owner }
            };

            return await  _dbHelper.ExecuteScalar<int>(query, parameters);
        }

        public async Task<int> DeleteAsync(int id)
        {
            string query = @"DELETE FROM projects WHERE id = @Id;";

            var parameters = new List<SqlParameter>
    {
        new SqlParameter("@Id", SqlDbType.Int) { Value = id }
    };

            return await _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
