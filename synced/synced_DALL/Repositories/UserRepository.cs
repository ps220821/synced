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

        public bool Login(string email, string password)
        {
            using (SqlConnection connection = _dbHelper.GetConnection())
            {
                connection.Open();

                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM users WHERE email = @email AND password = @password", connection);
                command.Parameters.AddWithValue("@email", email);
                command.Parameters.AddWithValue("@password", password);

                int userCount = (int)command.ExecuteScalar();

                if (userCount > 0)
                {
                    return true; 
                }
                else
                {
                    return false;
                }
            }

        }

        public bool Register(User user)  // Ensure it matches the interface
        {
            try
            {
                using (SqlConnection connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("INSERT INTO users (username, firstname, lastname, email, password, created_at) " +
                    "VALUES (@username, @firstname, @lastname, @email, @password, @created_at)",
                    connection);

                    command.Parameters.AddWithValue("@username", user.username);
                    command.Parameters.AddWithValue("@firstname", user.firstname);
                    command.Parameters.AddWithValue("@lastname", user.lastname);
                    command.Parameters.AddWithValue("@email", user.email);
                    command.Parameters.AddWithValue("@password", user.password);
                    command.Parameters.AddWithValue("@created_at", DateTime.Now);

                    int rowsAffected = command.ExecuteNonQuery();

                    connection.Close();

                    return rowsAffected > 0; // Return true if insert was successful
                }
            }
            catch (Exception ex)
            {
                throw new Exception(    ex.Message );
            }
            
        }
    }
}
