using System.Linq;
using Microsoft.AspNetCore.Identity.Test;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Wdpr_Groep_E.Controllers;
using Wdpr_Groep_E.Data;
using Wdpr_Groep_E.Hubs;
using Wdpr_Groep_E.Models;
using Xunit;

namespace Wdpr_Groep_E.Tests
{
    public class ChatTests
    {
        [Fact]
        public void CreateMessageTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("CreateMessageTestDb").Options);

            var controller = new ChatController(context, MockHelpers.MockUserManager<AppUser>().Object, new Mock<IHubContext<ChatHub>>().Object);

            context.Chats.Add(new Chat
            {
                Type = ChatType.Room,
                Name = "TestUser",
                Subject = "TestSubject",
                AgeGroup = "12-13"
            });
            context.SaveChanges();

            context.Messages.Add(new Message
            {
                ChatId = context.Chats.First().Id,
                Text = "TestMessage"
            });
            context.SaveChanges();

            // Assert
            Assert.Equal("TestMessage", context.Messages.First().Text);
        }

        // [Fact]
        // public void JoinRoomTest()
        // {
        //     // Arrange
        //     var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("JoinRoomTestDb").Options);

        //     var controller = new ChatController(context, MockHelpers.MockUserManager<AppUser>().Object, new Mock<IHubContext<ChatHub>>().Object);

        //     context.Chats.Add(new Chat
        //     {
        //         Type = ChatType.Room,
        //         Name = "TestUser",
        //         Subject = "TestSubject",
        //         AgeGroup = "12-13"
        //     });
        //     context.SaveChanges();

        //     string id = "1234";
        //     string chat = context.Chats.First().Id.ToString();

        //     // Act
        //     var result = controller.JoinRoom(id, chat).Result;
        //     var okResult = result as OkResult;

        //     // Assert
        //     Assert.NotNull(okResult);
        //     Assert.Equal(200, okResult.StatusCode);
        // }
    }
}
