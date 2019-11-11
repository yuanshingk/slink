using Microsoft.AspNetCore.Mvc;
using SLink.Models;
using SLink.Services;
using System;
using System.Threading.Tasks;

namespace SLink.APIControllers
{
    [ApiController]
    public class ShortLinkController : ControllerBase
    {
        private readonly IShortLinkService _shortLinkService;
        public ShortLinkController(IShortLinkService shortLinkService)
        {
            _shortLinkService = shortLinkService;
        }

        [HttpPost("api/shortlink")]
        public async Task<ActionResult<string>> CreateShortLink([FromForm]CreateShortLinkRequest request)
        {
            bool result = Uri.TryCreate(request.UrlString.Trim(), UriKind.Absolute, out var uri);
            if (result)
            {
                var unescapedUri = Uri.UnescapeDataString(uri.ToString());
                return await _shortLinkService.CreateShortLink(unescapedUri).ConfigureAwait(false);
            }

            return BadRequest();
        }

        [HttpGet("api/shortlink/{hashid}")]
        public async Task<ActionResult<string>> RetrieveOriginalUrl(string hashid)
        {
            var originalUrl = await _shortLinkService.GetOriginalUrl(hashid).ConfigureAwait(false);
            if (originalUrl != null)
            {
                return originalUrl;
            }

            return NotFound();
        }

        [HttpGet("{hashid}")]
        public async Task<ActionResult<string>> GotoShortLinkOriginalAddress(string hashid)
        {
            var originalUrl = await _shortLinkService.GetOriginalUrl(hashid).ConfigureAwait(false);
            if (originalUrl != null)
            {
                return Redirect(originalUrl);
            }

            return BadRequest();
        }
    }
}
