using HashidsNet;
using Microsoft.Extensions.Configuration;
using SLink.Providers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SLink.Services
{
    public interface IShortLinkService
    {
        Task<string> CreateShortLink(string url);
        Task<string> GetOriginalUrl(string hashid);
    }

    public class ShortLinkService : IShortLinkService
    {
        private readonly IDataProvider _dataProvider;
        private readonly IHashids _hashids;
        private readonly IConfiguration _configuration;
        
        public ShortLinkService(IConfiguration configuration, IDataProvider dataProvider, IHashids hashids)
        {
            _configuration = configuration;
            _dataProvider = dataProvider;
            _hashids = hashids;
        }

        public async Task<string> CreateShortLink(string url)
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                var existingUrlId = await _dataProvider.GetUrlId(url).ConfigureAwait(false);
                if (existingUrlId.HasValue)
                {
                    return ComputeShortLink(existingUrlId.Value);
                }

                var newUrlId = await _dataProvider.CreateUrlId(url).ConfigureAwait(false);
                if (newUrlId.HasValue)
                {
                    return ComputeShortLink(newUrlId.Value);
                }
            }

            return null;
        }

        public async Task<string> GetOriginalUrl(string hashid)
        {
            if (!string.IsNullOrWhiteSpace(hashid))
            {
                var urlId = _hashids.Decode(hashid);
                if (urlId != null && urlId.Any())
                {
                    return await _dataProvider.GetOriginalUrl(urlId.First());
                }
            }

            return null;
        }

        private string ComputeShortLink(int urlId)
        {
            var hash = _hashids.Encode(urlId);
            var baseUrl = _configuration.GetValue<string>("SLINK_BASE_URL");
            if (Uri.TryCreate(new Uri(baseUrl), hash, out var finalUrl))
            {
                return finalUrl.ToString();
            }

            return null;
        }
    }
}
