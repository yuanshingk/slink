using Microsoft.AspNetCore.Mvc;
using SLink.Controllers;
using Xunit;

namespace SLink.UnitTest.Controllers
{
    public class HomeControllerTest
    {
        [Fact]
        public void Index_ReturnViewResult()
        {
            var controller = new HomeController();
            var result = controller.Index() as ViewResult;
            Assert.NotNull(result);
        }
    }
}
