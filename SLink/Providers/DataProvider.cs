using SLink.Repositories;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SLink.Providers
{
    public interface IDataProvider
    {
        Task<int?> GetUrlId(string url);
        Task<int?> CreateUrlId(string url);
        Task<string> GetOriginalUrl(int urlId);
    }

    public class DataProvider : IDataProvider
    {
        private readonly IRepository _repository;

        public DataProvider(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<int?> GetUrlId(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string hash = ComputeMd5Hash(url);
                var records = await _repository.RetrieveUrlRecords(hash).ConfigureAwait(false);
                var matchingRecord = records?.FirstOrDefault(r => r.OriginalUrl == url);
                return matchingRecord?.Id;
            }

            return null;
        }

        public async Task<int?> CreateUrlId(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                string hash = ComputeMd5Hash(url);
                return await _repository.InsertUrlRecord(url, hash).ConfigureAwait(false);
            }

            return null;
        }

        public async Task<string> GetOriginalUrl(int urlId)
        {
            return await _repository.RetrieveUrl(urlId).ConfigureAwait(false);
        }

        private string ComputeMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}
