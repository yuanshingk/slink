using Moq;
using SLink.Models;
using SLink.Providers;
using SLink.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace SLink.UnitTest.Providers
{
    public class DataProviderTest
    {
        private const string URL = "https://dummy.com";
        private const string URL_MD5 = "40104EE8FC0D88F52E32F13F7D8E25E9";
        private Mock<IRepository> _repositoryMock;

        public DataProviderTest()
        {
            _repositoryMock = new Mock<IRepository>();
        }

        #region GetUrlId

        [Theory(DisplayName = "Invalid Input")]
        [InlineData("  ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetUrlId_EmptyUrlString_ReturnNull(string inputUrl)
        {
            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.GetUrlId(inputUrl).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task GetUrlId_SingleRecordInRepositoryMatchMD5Hash_ReturnRetrievedRecordId()
        {
            _repositoryMock.Setup(r => r
                .RetrieveUrlRecords(It.Is<string>(x => string.Compare(x, URL_MD5, StringComparison.OrdinalIgnoreCase) == 0)))
                .ReturnsAsync(new List<UrlRecord>
            {
                new UrlRecord { Id = 20, Hash = URL_MD5, OriginalUrl = URL, CreatedDate = DateTime.Now }
            });

            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.GetUrlId(URL).ConfigureAwait(false);

            Assert.True(result.HasValue);
            Assert.Equal(20, result.Value);
        }

        [Fact]
        public async Task GetUrlId_MultipleRecordsInRepositoryMatchMD5Hash_MatchUrlAndReturnId()
        {
            _repositoryMock.Setup(r => r
                .RetrieveUrlRecords(It.Is<string>(x => string.Compare(x, URL_MD5, StringComparison.OrdinalIgnoreCase) == 0)))
                .ReturnsAsync(new List<UrlRecord>
            {
                new UrlRecord { Id = 11, Hash = URL_MD5, OriginalUrl = "http://notthisurl.com", CreatedDate = DateTime.Now },
                new UrlRecord { Id = 20, Hash = URL_MD5, OriginalUrl = URL, CreatedDate = DateTime.Now }
            });

            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.GetUrlId(URL).ConfigureAwait(false);

            Assert.True(result.HasValue);
            Assert.Equal(20, result.Value);
        }

        [Fact]
        public async Task GetUrlId_NoRecordsMatchingUrlInRepository_ReturnNull()
        {
            _repositoryMock.Setup(r => r
                .RetrieveUrlRecords(It.Is<string>(x => string.Compare(x, URL_MD5, StringComparison.OrdinalIgnoreCase) == 0)))
                .ReturnsAsync(new List<UrlRecord>
            {
                new UrlRecord { Id = 11, Hash = URL_MD5, OriginalUrl = "http://notthisurl.com", CreatedDate = DateTime.Now },
                new UrlRecord { Id = 20, Hash = URL_MD5, OriginalUrl = "http://notthiseither.com", CreatedDate = DateTime.Now }
            });

            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.GetUrlId(URL).ConfigureAwait(false);

            Assert.Null(result);
        }

        #endregion

        #region CreateUrlId

        [Theory(DisplayName = "Invalid Input")]
        [InlineData("  ")]
        [InlineData("")]
        [InlineData(null)]
        public async Task CreateUrlId_EmptyUrlString_ReturnNull(string inputUrl)
        {
            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.CreateUrlId(inputUrl).ConfigureAwait(false);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUrlId_SuccessfullyInsertToRepository_ReturnNewId()
        {
            _repositoryMock.Setup(r => r
                .InsertUrlRecord(URL, It.Is<string>(x => string.Compare(x, URL_MD5, StringComparison.OrdinalIgnoreCase) == 0)))
                .ReturnsAsync(10);

            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.CreateUrlId(URL).ConfigureAwait(false);

            Assert.True(result.HasValue);
            Assert.Equal(10, result.Value);
        }

        #endregion

        #region GetOriginalUrl

        [Fact]
        public async Task GetOriginalUrl_RetrieveOriginalUrlFromProvider_ReturnOriginalUrl()
        {
            _repositoryMock.Setup(r => r.RetrieveUrl(100)).ReturnsAsync("http://dummy.com");

            var sut = new DataProvider(_repositoryMock.Object);
            var result = await sut.GetOriginalUrl(100).ConfigureAwait(false);

            Assert.Equal("http://dummy.com", result);
            _repositoryMock.VerifyAll();
        }

        #endregion
    }
}
