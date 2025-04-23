using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace synced_BBL.Interfaces.Helpers
{
    public interface IDatabaseHelper
    {
        SqlConnection GetConnection();

        Task<int> ExecuteNonQuery(string query, List<SqlParameter> parameters);

        Task<T> ExecuteScalar<T>(string query, List<SqlParameter> parameters);

        Task<List<T>> ExecuteReader<T>(string query, List<SqlParameter> parameters, Func<SqlDataReader, T> map);
    }
}
