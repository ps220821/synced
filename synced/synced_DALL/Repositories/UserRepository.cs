using synced_DALL.Entities;
using synced_DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Collections;
using synced.Core.Results;



namespace synced_DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository(DatabaseHelper dbhelper)
        {
            this._dbHelper = dbhelper;
        }

        public async Task<User> GetByMail(string email)
        {
            try
            {
                string query = @"
            SELECT id, username, firstname, lastname, email, password, created_at
            FROM users
            WHERE email = @Email;";

                var parameters = new List<SqlParameter>
        {
            new SqlParameter("@Email", SqlDbType.VarChar) { Value = email }
        };

                var users = await _dbHelper.ExecuteReader(query, parameters, reader =>
                {
                    int id = reader.GetInt32(reader.GetOrdinal("id"));
                    string username = reader.IsDBNull(reader.GetOrdinal("username"))
                                       ? null
                                       : reader.GetString(reader.GetOrdinal("username"));
                    string firstname = reader.IsDBNull(reader.GetOrdinal("firstname"))
                                       ? null
                                       : reader.GetString(reader.GetOrdinal("firstname"));
                    string lastname = reader.IsDBNull(reader.GetOrdinal("lastname"))
                                       ? null
                                       : reader.GetString(reader.GetOrdinal("lastname"));
                    string mail = reader.GetString(reader.GetOrdinal("email"));
                    string password = reader.GetString(reader.GetOrdinal("password"));
                    DateTime created = reader.GetDateTime(reader.GetOrdinal("created_at"));

                    return new User(
                        id,
                        username,
                        firstname,
                        lastname,
                        mail,
                        password,
                        created
                    );
                });

                return (users.Count > 0) ? users[0] : null;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error retrieving user by email.", ex);
            }
        }

        public async Task<int> CheckEmailExists(string email)
        {
            try
            {
                string query = "SELECT id FROM users WHERE email = @Email;";
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
                };

                return await _dbHelper.ExecuteScalar<int>(query, parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error checking email existence.", ex);
            }
        }

        public async Task<int> Register(User user)
        {
            try
            {
                string insertQuery = @"
                    INSERT INTO users (username, firstname, lastname, email, password, created_at)
                    VALUES (@Username, @Firstname, @Lastname, @Email, @Password, @CreatedAt);
                    SELECT SCOPE_IDENTITY();";

                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Username", SqlDbType.VarChar) { Value = user.Username ?? (object)DBNull.Value },
                    new SqlParameter("@Firstname", SqlDbType.VarChar) { Value = user.Firstname ?? (object)DBNull.Value },
                    new SqlParameter("@Lastname", SqlDbType.VarChar) { Value = user.Lastname ?? (object)DBNull.Value },
                    new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.Email },
                    new SqlParameter("@Password", SqlDbType.VarChar) { Value = user.PasswordHash },
                    new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = user.CreatedAt }
                };

                return await _dbHelper.ExecuteScalar<int>(insertQuery, parameters);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Error registering user.", ex);
            }
        }
    }
}