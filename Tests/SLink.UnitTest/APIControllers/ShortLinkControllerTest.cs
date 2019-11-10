using Microsoft.AspNetCore.Mvc;
using Moq;
using SLink.APIControllers;
using SLink.Models;
using SLink.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
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
            _shortLinkServiceMock.Setup(s => s.CreateShortLink(expectedUrl)).Returns(Task.FromResult(expectedUrl));
            
            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.CreateShortLink(new CreateShortLinkRequest { UrlString = actualUrl });

            _shortLinkServiceMock.VerifyAll();
            Assert.Equal(expectedUrl, result.Value);
        }

        [Fact]
        public async Task CreateShortLink_InvalidUrlFormat_ReturnBadRequestResponse()
        {
            var sut = new ShortLinkController(_shortLinkServiceMock.Object);
            var result = await sut.CreateShortLink(new CreateShortLinkRequest { UrlString = "xyz" });

            var statusCodeResult = result.Result as StatusCodeResult;
            Assert.Equal(400, statusCodeResult.StatusCode);
            _shortLinkServiceMock.Verify(s => s.CreateShortLink(It.IsAny<string>()), Times.Never);
        }
    }
}
