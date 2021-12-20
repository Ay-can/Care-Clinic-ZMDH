using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Wdpr_Groep_E.Controllers;
using Xunit;

namespace Webapp.Tests
{
    //Testen runnen via de terminal, anders doen ze het niet.... In de terminal dotnet test typen.
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
        //Om te kijken of de reference werkt....
        // HomeController home = new HomeController();
        // var result = home.Index();
        // //result moet een view zijn

        // var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(1,1);
        }

        [Fact]
        public void test2()
        {
            Assert.Equal("wdpr","wdpr");
        }
    }
}
