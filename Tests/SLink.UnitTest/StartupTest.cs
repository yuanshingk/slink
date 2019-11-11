using HashidsNet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SLink.Providers;
using SLink.Repositories;
using SLink.Services;
using Xunit;

namespace SLink.UnitTest
{
    public class StartupTest
    {
        [Fact]
        public void Startup_ServiceDependenciesAreRegistered()
        {
            var webHost = Microsoft.AspNetCore.WebHost.CreateDefaultBuilder().UseStartup<Startup>().Build();
           
            Assert.NotNull(webHost);
            Assert.NotNull(webHost.Services.GetRequiredService<IShortLinkService>());
            Assert.NotNull(webHost.Services.GetRequiredService<IDataProvider>());
            Assert.NotNull(webHost.Services.GetRequiredService<IRepository>());
            Assert.NotNull(webHost.Services.GetRequiredService<IHashids>());
        }
    }
}
