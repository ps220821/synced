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

        public async Task<int> Login(string email, string password)
        {
            
                string query = "SELECT id FROM users WHERE email = @Email AND password = @Password";
                var parameters = new List<SqlParameter>
                {
                    new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
                    new SqlParameter("@Password", SqlDbType.VarChar) { Value = password }
                };

                return await _dbHelper.ExecuteScalar<int>(query, parameters);
        }

        public async Task<int> CheckEmailExists(string email)
        {
            string query = "SELECT id FROM users WHERE email = @Email;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
            };


            return await _dbHelper.ExecuteScalar<int>(query, parameters);
        }

        public async Task<int> Register(User user)
        {
            string insertQuery = "INSERT INTO users (username, firstname, lastname, email, password, created_at) " +
                                 "VALUES (@Username, @Firstname, @Lastname, @Email, @Password, @CreatedAt)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@Username", SqlDbType.VarChar) { Value = user.Username },
                new SqlParameter("@Firstname", SqlDbType.VarChar) { Value = user.Firstname },
                new SqlParameter("@Lastname", SqlDbType.VarChar) { Value = user.Lastname },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.Email },
                new SqlParameter("@Password", SqlDbType.VarChar) { Value = user.Password },
                new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = DateTime.Now }
            };
            return await _dbHelper.ExecuteScalar<int>(insertQuery, insertParameters);
        }
    }
}