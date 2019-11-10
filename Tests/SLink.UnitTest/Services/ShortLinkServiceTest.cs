using HashidsNet;
using Moq;
using SLink.Repositories;
using SLink.Services;
using System.Threading.Tasks;
using Xunit;

namespace SLink.UnitTest.Services
{
    public class ShortLinkServiceTest
    {
        private Mock<IRepository> _repositoryMock;
        private Mock<IHashids> _hashidsMock;

        public ShortLinkServiceTest()
        {
            _repositoryMock = new Mock<IRepository>();
            _hashidsMock = new Mock<IHashids>();
        }

        [Theory]
        [InlineData("  ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateShortLink_InvalidInputUrl_ReturnNull(string inputUrl)
        {
            var sut = new ShortLinkService(_repositoryMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink(inputUrl).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateSHortLink_UrlExistingInRepository_ReturnShortLinkWithoutInsertToRepository()
        {
            _repositoryMock.Setup(x => x.GetUrlId(It.IsAny<string>())).ReturnsAsync(10);
            _hashidsMock.Setup(x => x.Encode(10)).Returns("XYUNWEGFIH");

            var sut = new ShortLinkService(_repositoryMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink("https://dummy.com").ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal("https://slinkweb.azurewebsites.net/XYUNWEGFIH", result);
            _repositoryMock.Verify(x => x.InsertUrlRecord(It.IsAny<string>()), Times.Never);
            _hashidsMock.VerifyAll();
        }

        [Fact]
        public async Task CreateSHortLink_UrlNotFoundInRepository_ReturnShortLinkAndInsertToRepository()
        {
            _repositoryMock.Setup(x => x.GetUrlId(It.IsAny<string>())).ReturnsAsync((int?)null);
            _repositoryMock.Setup(x => x.InsertUrlRecord("https://dummy.com")).ReturnsAsync(100);
            _hashidsMock.Setup(x => x.Encode(100)).Returns("U78BCWEGFIH");

            var sut = new ShortLinkService(_repositoryMock.Object, _hashidsMock.Object);
            var result = await sut.CreateShortLink("https://dummy.com").ConfigureAwait(false);

            Assert.NotNull(result);
            Assert.Equal("https://slinkweb.azurewebsites.net/U78BCWEGFIH", result);
            _repositoryMock.VerifyAll();
            _hashidsMock.VerifyAll();
        }
    }
}
