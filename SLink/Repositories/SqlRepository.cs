using System;
using System.Threading.Tasks;

namespace SLink.Repositories
{
    public interface IRepository
    {
        Task<int?> GetUrlId(string url);
        Task<int> InsertUrlRecord(string url);
    }

    public class SqlRepository : IRepository
    {
        public async Task<int?> GetUrlId(string url)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertUrlRecord(string url)
        {
            throw new NotImplementedException();
        }
    }
}
