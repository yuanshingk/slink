using Microsoft.AspNetCore.Mvc;
using slink.Models;
using System;

namespace slink.APIControllers
{
    [ApiController]
    [Route("api/shortlink")]
    public class ShortLinkController : ControllerBase
    {
        [HttpPost]
        public ActionResult<string> CreateShortLink([FromForm]CreateShortLinkRequest request)
        {
            try
            {
                // TODO: implementation 
                var uri = new Uri(request.UrlString.Trim().ToLower());
                return uri.ToString();
            }
            catch (UriFormatException)
            {
                return BadRequest();
            }
        }
    }
}
