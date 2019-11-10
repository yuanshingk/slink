using HashidsNet;
using SLink.Repositories;
using System;
using System.Threading.Tasks;

namespace SLink.Services
{
    public interface IShortLinkService
    {
        Task<string> CreateShortLink(string url);
    }

    public class ShortLinkService : IShortLinkService
    {
        private const string SLINK_BASE_URL = "https://slinkweb.azurewebsites.net/"; //TEMP
        private readonly IRepository _sqlRepository;
        private readonly IHashids _hashids;
        
        public ShortLinkService(IRepository sqlRepository, IHashids hashids)
        {
            _sqlRepository = sqlRepository;
            _hashids = hashids;
        }

        public async Task<string> CreateShortLink(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                var existingUrlId = await _sqlRepository.GetUrlId(url).ConfigureAwait(false);
                if (existingUrlId.HasValue)
                {
                    return ComputeShortLink(existingUrlId.Value);
                }

                var newUrlId = await _sqlRepository.InsertUrlRecord(url).ConfigureAwait(false);
                return ComputeShortLink(newUrlId);
            }

            return null;
        }

        private string ComputeShortLink(int urlId)
        {
            var hash = _hashids.Encode(urlId);
            if (Uri.TryCreate(new Uri(SLINK_BASE_URL), hash, out var finalUrl))
            {
                return finalUrl.ToString();
            }

            return null;
        }
    }
}
