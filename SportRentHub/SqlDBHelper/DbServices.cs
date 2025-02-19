using Microsoft.Data.SqlClient;
using Dapper;
using Serilog;
namespace SportRentHub.SqlDBHelper
{
    public class DbServices : IDbServices
    {
        private readonly string _connectionString;
        public DbServices(IConfiguration configuration)
        {

            _connectionString = configuration.GetValue<string>("ConnectionStrings:SportRentHub", "localhost:7110");
        }
        public async Task<int> EditData(string command, object parms)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return await connection.ExecuteAsync(command, parms);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in EditData. Command: {Command}, Params: {@Params}", command, parms);
                throw;
            }
        }

        public async Task<List<T>> GetAll<T>(string command, object parms)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return (await connection.QueryAsync<T>(command, parms)).ToList();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetAll. Command: {Command}, Params: {@Params}", command, parms);
                throw;
            }
        }

        public async Task<T> GetAsync<T>(string command, object parms)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    return (await connection.QueryAsync<T>(command, parms).ConfigureAwait(false)).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in GetAsync. Command: {Command}, Params: {@Params}", command, parms);
                throw;
            }

        }
    }
}
