using synced_DAL.Entities;
using synced_DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;


namespace synced_DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public UserRepository(DatabaseHelper dbhelper)
        {
            this._dbHelper = dbhelper;
        }

        public  bool Login(User user)
        {
            string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password";


            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@username", user.username);
                command.Parameters.AddWithValue("@password", user.password);


                int userCount = (int)command.ExecuteScalar();

                // If userCount > 0, login is successful
                if (userCount > 0)
                {
                    return true;  // Login successful
                }
                else
                {
                    return false; // Invalid username/password
                }
            }

        }

        public bool Register(User user)
        {
            throw new NotImplementedException();
        }
    }
}
