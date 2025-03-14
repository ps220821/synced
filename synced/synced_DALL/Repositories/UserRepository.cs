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
            try
            {
                using (SqlConnection connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // Use SQL parameters to protect against SQL injection
                    SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM users WHERE email = @Email AND password = @Password", connection);
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = email });
                    command.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = password });

                    int userCount = (int)command.ExecuteScalar();

                    return userCount > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("There was an issue with the database. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unknown error occurred.", ex);
            }
        }


        public bool Register(User user)
        {
            try
            {
                using (SqlConnection connection = _dbHelper.GetConnection())
                {
                    connection.Open();

                    // Use SQL parameters to protect against SQL injection
                    SqlCommand command = new SqlCommand("INSERT INTO users (username, firstname, lastname, email, password, created_at) " +
                        "VALUES (@Username, @Firstname, @Lastname, @Email, @Password, @CreatedAt)", connection);

                    command.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar) { Value = user.username });
                    command.Parameters.Add(new SqlParameter("@Firstname", SqlDbType.VarChar) { Value = user.firstname });
                    command.Parameters.Add(new SqlParameter("@Lastname", SqlDbType.VarChar) { Value = user.lastname });
                    command.Parameters.Add(new SqlParameter("@Email", SqlDbType.VarChar) { Value = user.email });
                    command.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar) { Value = user.password });
                    command.Parameters.Add(new SqlParameter("@CreatedAt", SqlDbType.DateTime) { Value = DateTime.Now });

                    int rowsAffected = command.ExecuteNonQuery();

                    return rowsAffected > 0;
                }
            }
            catch (SqlException ex)
            {
                throw new Exception("There was an issue with the database. Please try again later.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An unknown error occurred during registration.", ex);
            }
        }
    }
}