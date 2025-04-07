﻿using Microsoft.Data.SqlClient;

namespace synced_DAL
{
    public class DatabaseHelper
    {
        private readonly string _connectionString;

        public DatabaseHelper(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection GetConnection()
        {
            var connection = new SqlConnection(_connectionString);
            return connection;
        }

        public int ExecuteNonQuery(string query, List<SqlParameter> parameters)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        return command.ExecuteNonQuery() ;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Gebruik direct de method om de fout te vertalen
                throw new DatabaseException(DatabaseHelper.GetErrorMessage(ex), ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Unexpected error occurred while executing query.", ex);
            }
        }

        public int ExecuteScalar(string query, List<SqlParameter> parameters)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        object result = command.ExecuteScalar();
                        return (result != null && result != DBNull.Value) ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                // Gebruik direct de method om de fout te vertalen
                throw new DatabaseException(DatabaseHelper.GetErrorMessage(ex), ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Unexpected error occurred while executing query.", ex);
            }
        }

        public List<T> ExecuteReader<T>(string query, List<SqlParameter> parameters, Func<SqlDataReader, T> map)
        {
            try
            {
                using (SqlConnection connection = GetConnection())
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddRange(parameters.ToArray());
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            List<T> results = new List<T>();
                            while (reader.Read())
                            {
                                results.Add(map(reader)); // Use the provided mapping function
                            }
                            return results;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new DatabaseException(DatabaseHelper.GetErrorMessage(ex), ex);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Unexpected error occurred while executing query.", ex);
            }
        }

        public static string GetErrorMessage(SqlException ex)
        {
            return ex.Number switch
            {
                2 or 53 or 11001 => "Cannot connect to the database server. Please try again later.",
                4060 => "The requested database is not available.",
                18456 => "Invalid database username or password.",
                547 => "This action cannot be completed due to related data dependencies.",
                2627 or 2601 => "The email is already in use. Please choose a different one.",
                1205 => "A database conflict occurred. Please try again.",
                233 => "Cannot connect to the database. Please check your internet connection or try again later.",
                _ => "An unexpected database error occurred. Please try again later."
            };
        }
    }



    // Custom exception for database errors
    public class DatabaseException : Exception
    {
        public DatabaseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
