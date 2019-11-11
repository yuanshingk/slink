using HashidsNet;
using Moq;
using SLink.Providers;
using SLink.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SLink.UnitTest.Services
{
    public class ShortLinkServiceTest
    {
        private Mock<IDataProvider> _dataProviderMock;
        private Mock<IHashids> _hashidsMock;

        public ShortLinkServiceTest()
        {
            _dataProviderMock = new Mock<IDataProvider>();
            _hashidsMock = new Mock<IHashids>();
            Environment.SetEnvironmentVariable("SLINK_BASE_URL", "https://slinkweb.azurewebsites.net/");
        }

        #region CreateShortLink

        [Theory(DisplayName = "Invalid Input")]
        [InlineData("  ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateShortLink_InvalidInputUrl_ReturnNull(string inputUrl)
        {
            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink(inputUrl).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateSHortLink_DataProviderReturnExistingId_ReturnShortLink()
        {
            _dataProviderMock.Setup(x => x.GetUrlId(It.IsAny<string>())).ReturnsAsync(10);
            _hashidsMock.Setup(x => x.Encode(10)).Returns("XYUNWEGFIH");

            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink("https://dummy.com").ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal("https://slinkweb.azurewebsites.net/XYUNWEGFIH", result);
            _dataProviderMock.Verify(x => x.CreateUrlId(It.IsAny<string>()), Times.Never);
            _hashidsMock.VerifyAll();
        }

        [Fact]
        public async Task CreateSHortLink_DataProviderDoesNotReturnId_CreateUrlIdAndReturnShortLink()
        {
            _dataProviderMock.Setup(x => x.GetUrlId(It.IsAny<string>())).ReturnsAsync((int?)null);
            _dataProviderMock.Setup(x => x.CreateUrlId("https://dummy.com")).ReturnsAsync(100);
            _hashidsMock.Setup(x => x.Encode(100)).Returns("U78BCWEGFIH");

            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink("https://dummy.com").ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal("https://slinkweb.azurewebsites.net/U78BCWEGFIH", result);
            _dataProviderMock.VerifyAll();
            _hashidsMock.VerifyAll();
        }

        [Fact]
        public async Task CreateSHortLink_DataProviderFailToCreateNewId_ReturnNull()
        {
            _dataProviderMock.Setup(x => x.GetUrlId(It.IsAny<string>())).ReturnsAsync((int?)null);
            _dataProviderMock.Setup(x => x.CreateUrlId("https://dummy.com")).ReturnsAsync((int?)null);

            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink("https://dummy.com").ConfigureAwait(false);

            Assert.Null(result);
        }

        #endregion

        #region GetOriginalUrl

        [Theory(DisplayName = "Invalid Input")]
        [InlineData("  ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetOriginalUrl_InvalidHashInput_ReturnNull(string inputHash)
        {
            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.GetOriginalUrl(inputHash).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOriginalUrl_DecoderReturnsNoUrlId_ReturnNull()
        {
            _hashidsMock.Setup(x => x.Decode("FDHFHH")).Returns(new int[0]);

            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.GetOriginalUrl("FDHFHH").ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOriginalUrl_DecoderReturnsNull_ReturnNull()
        {
            _hashidsMock.Setup(x => x.Decode("FDHFHH")).Returns((int[])null);

            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.GetOriginalUrl("FDHFHH").ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetOriginalUrl_DecoderReturnsUrlIds_ReturnOriginalUrlOfTheFirstUrlId()
        {
            _hashidsMock.Setup(x => x.Decode("FDHFHH")).Returns(new int[] { 1, 2, 3});
            _dataProviderMock.Setup(x => x.GetOriginalUrl(1)).ReturnsAsync("https://dummy.com");

            var sut = new ShortLinkService(_dataProviderMock.Object, _hashidsMock.Object);
            var result = await sut.GetOriginalUrl("FDHFHH").ConfigureAwait(false);

            Assert.Equal("https://dummy.com", result);
        }

        #endregion
    }
}
