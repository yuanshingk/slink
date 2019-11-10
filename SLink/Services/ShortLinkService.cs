using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SLink.Services
{
    public interface IShortLinkService
    {
        Task<string> CreateShortLink(string input);
    }

    public class ShortLinkService : IShortLinkService
    {
        public async Task<string> CreateShortLink(string input)
        {
            throw new NotImplementedException();
        }
    }
}
