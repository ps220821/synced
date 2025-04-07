using Microsoft.Data.SqlClient;
using synced_DAL;
using synced_DAL.Entities;
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
        public List<Project> GetAllAsync(int id)
        {
            string query = @"
                SELECT p.id AS project_id, pu.user_id, p.owner, p.name, p.description, p.start_date, p.end_date 
                FROM project_users pu 
                JOIN projects p ON pu.project_id = p.id 
                WHERE pu.user_id = @id;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@id", SqlDbType.Int) { Value = id }
            };

            return _dbHelper.ExecuteReader(query, parameters, reader =>
            {
                return new Project
                {
                    Id = reader.GetInt32(reader.GetOrdinal("project_id")),
                    Name = reader.GetString(reader.GetOrdinal("name")),
                    Description = reader.GetString(reader.GetOrdinal("description")),
                    Start_Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("start_date"))),
                    End_Date = reader.IsDBNull(reader.GetOrdinal("end_date"))
                        ? default
                        : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("end_date"))),
                    Owner = reader.GetInt32(reader.GetOrdinal("owner"))
                };
            });
        }

        public Task<Project> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public int CreateAsync(Project project)
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

            return _dbHelper.ExecuteScalar(query, parameters);
        }

        public bool UpdateAsync(Project entity)
        {
            throw new NotImplementedException();
        }

        public int DeleteAsync(int id)
        {
            string query = @"DELETE FROM projects WHERE id = @Id;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            };

            return _dbHelper.ExecuteNonQuery(query, parameters);
        }
    }
}
