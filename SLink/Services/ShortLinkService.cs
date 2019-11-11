using HashidsNet;
using SLink.Providers;
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
        private readonly IDataProvider _dataProvider;
        private readonly IHashids _hashids;
        
        public ShortLinkService(IDataProvider dataProvider, IHashids hashids)
        {
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
