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



namespace synced_DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository(DatabaseHelper dbhelper)
        {
            this._dbHelper = dbhelper;
        }

        public int  GetUserByEmail(string email)
        {
            string query = "SELECT id FROM users WHERE email = @Email;";


            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
            };

            int userId = _dbHelper.ExecuteScalar(query, parameters);

            return userId;
        }

        

        public int Login(string email, string password)
        {
            string query = "SELECT id FROM users WHERE email = @Email AND password = @Password";

            var parameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = email },
                new SqlParameter("@Password", SqlDbType.VarChar) { Value = password }
            };

            int userId = _dbHelper.ExecuteScalar(query, parameters);

            return userId; // Returns the user_id if login is successful, otherwise 0
        }

        public bool Register(User user)
        {
            string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email";
            var checkEmailParameters = new List<SqlParameter>
            {
                new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.email }
            };

            int existingUserCount = _dbHelper.ExecuteScalar(checkEmailQuery, checkEmailParameters);

            if (existingUserCount > 0)
            {
                return false; 
            }


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

            return _dbHelper.ExecuteNonQuery(insertQuery, insertParameters);
        }
    }
}