using System.Linq;
using FluentEmail.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wdpr_Groep_E.Controllers;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Models;
using Xunit;

namespace Wdpr_Groep_E.Tests
{
    public class ChatSystemTests
    {
        [Fact]
        public void CreateRoomTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("CreateRoomTestDb").Options);

            var controller = new ChatSystemController(
                new Mock<IFluentEmail>().Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Chats.Add(new Chat
            {
                Type = ChatType.Room,
                Name = "TestUser",
                Subject = "TestSubject",
                AgeGroup = "12-13"
            });
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Chats.Where(c => c.Type == ChatType.Room).Count());
        }

        [Fact]
        public void CreatePrivateRoomTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("CreatePrivateRoomTestDb").Options);

            var controller = new ChatSystemController(
                new Mock<IFluentEmail>().Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Chats.Add(new Chat
            {
                Type = ChatType.Private,
                Name = "TestUser"
            });
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Chats.Where(c => c.Type == ChatType.Private).Count());
        }

        [Fact]
        public void JoinRoomTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("JoinRoomTestDb").Options);

            var controller = new ChatSystemController(
                new Mock<IFluentEmail>().Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Users.Add(new AppUser { UserName = "TestUser" });
            context.SaveChanges();

            context.Chats.Add(new Chat
            {
                Type = ChatType.Room,
                Name = context.Users.First().UserName
            });
            context.SaveChanges();

            context.ChatUsers.Add(new ChatUser
            {
                ChatId = 1,
                UserId = context.Users.First().Id
            });
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.ChatUsers.Where(cu => cu.ChatId == 1).Count());
        }

        [Fact]
        public void JoinPrivateRoomTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("JoinPrivateRoomTestDb").Options);

            var controller = new ChatSystemController(
                new Mock<IFluentEmail>().Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Users.Add(new AppUser { UserName = "TestUser" });
            context.SaveChanges();

            context.Chats.Add(new Chat
            {
                Type = ChatType.Private,
                Name = context.Users.First().UserName
            });
            context.SaveChanges();

            context.ChatUsers.Add(new ChatUser
            {
                ChatId = 1,
                UserId = context.Users.First().Id
            });
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.ChatUsers.Where(cu => cu.ChatId == 1).Count());
        }

        [Fact]
        public void BlockUserTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("BlockUserTestDb").Options);

            var controller = new ChatSystemController(
                new Mock<IFluentEmail>().Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Users.Add(new AppUser { UserName = "TestUser" });
            context.SaveChanges();

            context.Chats.Add(new Chat
            {
                Type = ChatType.Private,
                Name = context.Users.First().UserName,
            });
            context.SaveChanges();

            context.ChatUsers.Add(new ChatUser
            {
                ChatId = 1,
                UserId = context.Users.First().Id
            });
            context.SaveChanges();

            // Act
            context.ChatUsers.Where(cu => cu.ChatId == 1).First().IsBlocked = true;

            // Assert
            Assert.True(context.ChatUsers.Where(cu => cu.ChatId == 1).First().IsBlocked);
        }

        [Fact]
        public void UnBlockUserTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("UnBlockUserTestDb").Options);

            var controller = new ChatSystemController(
                new Mock<IFluentEmail>().Object, context, MockHelpers.MockUserManager<AppUser>().Object, MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Users.Add(new AppUser { UserName = "TestUser" });
            context.SaveChanges();

            context.Chats.Add(new Chat
            {
                Type = ChatType.Private,
                Name = context.Users.First().UserName,
            });
            context.SaveChanges();

            context.ChatUsers.Add(new ChatUser
            {
                ChatId = 1,
                UserId = context.Users.First().Id
            });
            context.SaveChanges();

            // Act
            context.ChatUsers.Where(cu => cu.ChatId == 1).First().IsBlocked = false;

            // Assert
            Assert.False(context.ChatUsers.Where(cu => cu.ChatId == 1).First().IsBlocked);
        }
    }
}
