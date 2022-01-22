using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wdpr_Groep_E.Controllers;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;
using Xunit;

namespace Wdpr_Groep_E.Tests
{
    public class ChatTests
    {
        private Mock<UserManager<AppUser>> GetMockUserManager()
        {
            var userStoreMock = new Mock<IUserStore<AppUser>>();
            return new Mock<UserManager<AppUser>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<RoleManager<IdentityRole>> GetMockRoleManager()
        {
            var roleManagerMock = new Mock<IUserStore<IdentityRole>>();
            return new Mock<RoleManager<IdentityRole>>(roleManagerMock.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public void CreateRoomTest()
        {
            DbContextOptions<WdprContext> options = new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("CreateTestDb").Options;

            var emailMock = new Mock<IFluentEmail>();
            var context = new WdprContext(options);
            var userManagerMock = GetMockUserManager();
            var roleManagerMock = GetMockRoleManager();


            var name = "TestUser";
            var age = "12-13";
            var subject = "TestSubject";

            var chatSystemController = new ChatSystemController(emailMock.Object, context, userManagerMock.Object, roleManagerMock.Object);

            var result = chatSystemController.CreateRoom(name, age, subject);

            Assert.Equal(1, context.Chats.CountAsync().Result);
        }
    }
}
