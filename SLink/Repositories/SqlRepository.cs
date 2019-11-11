using Microsoft.Extensions.Configuration;
using SLink.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SLink.Repositories
{
    public interface IRepository
    {
        Task<List<UrlRecord>> RetrieveUrlRecords(string hash);
        Task<int> InsertUrlRecord(string url, string hash);
        Task<string> RetrieveUrl(int urlId);
    }

    [ExcludeFromCodeCoverage]
    public class SqlRepository : IRepository
    {
        private readonly string slinkDbConnectionString;

        public SqlRepository(IConfiguration configuration)
        {
            slinkDbConnectionString = configuration.GetValue<string>("SLINKDB_CONNECTIONSTRING");
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

        public async Task<string> RetrieveUrl(int urlId)
        {
            string commandText = "SELECT * FROM dbo.UrlRecords WHERE Id=@Id";
            using (var connection = new SqlConnection(slinkDbConnectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                using (SqlCommand command = new SqlCommand(commandText, connection))
                {
                    command.Parameters.AddWithValue("@Id", urlId);
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            return reader.GetFieldValue<string>(reader.GetOrdinal("OriginalUrl"));
                        }
                    }
                }
            }

            return null;
        }
    }
}
