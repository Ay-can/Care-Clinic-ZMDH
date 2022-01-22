using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wdpr_Groep_E.Controllers;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;
using Xunit;
using Xunit.Abstractions;

namespace Wdpr_Groep_E.Tests
{
    public class ModeratorTests
    {
        [Fact]
        public void ClientInfoTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("ClientInfoTestDb").Options);

            var api = new Mock<IZmdhApi>();
            api.Setup(s => s.GetClientObject("10250").Result);
            var controller = new ClientController(api.Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object);

            // Act
            var result = controller.ClientInfo("10250");

            // Assert
            Assert.IsType<ViewResult>(result);
            api.Verify(s => s.GetClientObject("10250"), Times.Once);
        }
    }
}
