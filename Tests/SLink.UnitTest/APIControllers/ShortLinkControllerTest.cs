using Microsoft.AspNetCore.Mvc;
using Moq;
using SLink.APIControllers;
using SLink.Models;
using SLink.Services;
using System.Threading.Tasks;
using Xunit;

namespace SLink.UnitTest.APIControllers
{
    public class ShortLinkControllerTest
    {
        private Mock<IShortLinkService> _shortLinkServiceMock;

        public ShortLinkControllerTest()
        {
            _shortLinkServiceMock = new Mock<IShortLinkService>();
        }

        [Theory(DisplayName = "Transform to standard format")]
        [InlineData("http://GOOGLE.com/x-x-x/y_y_y/zzz/", "http://google.com/x-x-x/y_y_y/zzz/")]
        [InlineData("   http://www.google.com   ", "http://www.google.com/")]
        [InlineData("http://www.google.com/search?term=What day is today?answer=I Don't know", "http://www.google.com/search?term=What day is today?answer=I Don't know")]
        [InlineData("http://www.google.com/search?term=What%20day%20is%20today%3Fanswer%3DI%20Don%27t%20know", "http://www.google.com/search?term=What day is today?answer=I Don't know")]
        public async Task CreateShortLink_ValidUrlFormat_CallShortLinkService(string actualUrl, string expectedUrl)
        {
            _shortLinkServiceMock.Setup(s => s.CreateShortLink(expectedUrl)).ReturnsAsync(expectedUrl);
            
            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.CreateShortLink(new CreateShortLinkRequest { UrlString = actualUrl }).ConfigureAwait(false);

            _shortLinkServiceMock.VerifyAll();
            Assert.Equal(expectedUrl, result.Value);
        }

        [Fact]
        public async Task CreateShortLink_InvalidUrlFormat_ReturnBadRequestResponse()
        {
            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.CreateShortLink(new CreateShortLinkRequest { UrlString = "xyz" }).ConfigureAwait(false);

            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.Equal(400, statusCodeResult.StatusCode);
            _shortLinkServiceMock.Verify(s => s.CreateShortLink(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task RetrieveOriginalUrl_HashIdIsMatchingRecords_ReturnOriginalUrl()
        {
            _shortLinkServiceMock.Setup(s => s.GetOriginalUrl("XUDH")).ReturnsAsync("https://dummy.com");

            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.RetrieveOriginalUrl("XUDH").ConfigureAwait(false);

            _shortLinkServiceMock.VerifyAll();
            Assert.Equal("https://dummy.com", result.Value);
        }

        [Fact]
        public async Task RetrieveOriginalUrl_HashIdIsNotMatchingRecords_ReturnNotFound()
        {
            _shortLinkServiceMock.Setup(s => s.GetOriginalUrl(It.IsAny<string>())).ReturnsAsync((string)null);

            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.RetrieveOriginalUrl("XUDH").ConfigureAwait(false);

            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.Equal(404, statusCodeResult.StatusCode);
        }


        [Fact]
        public async Task GotoShortLinkOriginalAddress_HashIdIsMatchingRecords_RedirectToUrlAddress()
        {
            _shortLinkServiceMock.Setup(s => s.GetOriginalUrl("XUDH")).ReturnsAsync("https://dummy.com");

            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.GotoShortLinkOriginalAddress("XUDH").ConfigureAwait(false);

            _shortLinkServiceMock.VerifyAll();
            var redirectResult = result.Result as RedirectResult;
            Assert.NotNull(redirectResult);
            Assert.Equal("https://dummy.com", redirectResult.Url);
        }

        [Fact]
        public async Task GotoShortLinkOriginalAddress_HashIdIsNotMatchingRecords_ReturnBadRequest()
        {
            _shortLinkServiceMock.Setup(s => s.GetOriginalUrl(It.IsAny<string>())).ReturnsAsync((string)null);

            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.GotoShortLinkOriginalAddress("XUDH").ConfigureAwait(false);

            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.Equal(400, statusCodeResult.StatusCode);
        }
    }
}
