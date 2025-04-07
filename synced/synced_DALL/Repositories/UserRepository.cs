using synced_DAL.Entities;
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

                return (int) _dbHelper.ExecuteScalar(query, parameters);
        }

        public async Task<int> CheckEmailExists(string email)
        {
            string query = "SELECT id FROM users WHERE email = @Email;";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
            };


            return (int) _dbHelper.ExecuteScalar(query, parameters);
        }

        public async Task<int> Register(User user)
        {
            string insertQuery = "INSERT INTO users (username, firstname, lastname, email, password, created_at) " +
                                 "VALUES (@Username, @Firstname, @Lastname, @Email, @Password, @CreatedAt)";
            var insertParameters = new List<SqlParameter>
            {
                new SqlParameter("@Username", SqlDbType.VarChar) { Value = user.username },
                new SqlParameter("@Firstname", SqlDbType.VarChar) { Value = user.firstname },
                new SqlParameter("@Lastname", SqlDbType.VarChar) { Value = user.lastname },
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.email },
                new SqlParameter("@Password", SqlDbType.VarChar) { Value = user.password },
                new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = DateTime.Now }
            };
            return (int)_dbHelper.ExecuteScalar(insertQuery, insertParameters);
        }
    }
}