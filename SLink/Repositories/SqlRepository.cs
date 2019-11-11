using SLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace SLink.Repositories
{
    public interface IRepository
    {
        Task<List<UrlRecord>> RetrieveUrlRecords(string hash);
        Task<int> InsertUrlRecord(string url, string hash);
    }

    public class SqlRepository : IRepository
    {
        private readonly string slinkDbConnectionString;

        public SqlRepository()
        {
            slinkDbConnectionString = Environment.GetEnvironmentVariable("SLINKDB_CONNECTIONSTRING");
        }

        public async Task<List<UrlRecord>> RetrieveUrlRecords(string hash)
        {
            var records = new List<UrlRecord>();
            string commandText = "SELECT * FROM dbo.UrlRecords WHERE Hash=@Hash";

            using (var connection = new SqlConnection(slinkDbConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Hash", hash);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            records.Add(new UrlRecord
                            {
                                Id = reader.GetFieldValue<int>(reader.GetOrdinal("Id")),
                                Hash = reader.GetFieldValue<string>(reader.GetOrdinal("Hash")),
                                OriginalUrl = reader.GetFieldValue<string>(reader.GetOrdinal("OriginalUrl")),
                                CreatedDate = reader.GetFieldValue<DateTime>(reader.GetOrdinal("CreatedDate"))
                            });
                        }
                    }
                }
            }

            return records;
        }

        public async Task<int> InsertUrlRecord(string url, string hash)
        {
            string commandText = "INSERT INTO dbo.UrlRecords (Hash, OriginalUrl) VALUES (@Hash, @OriginalUrl); SELECT SCOPE_IDENTITY()";
            using (var connection = new SqlConnection(slinkDbConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Hash", hash);
                    command.Parameters.AddWithValue("@OriginalUrl", url);
                    var commandOutput = await command.ExecuteScalarAsync().ConfigureAwait(false);
                    return Convert.ToInt32(commandOutput);
                }
            }
        }
    }
}
