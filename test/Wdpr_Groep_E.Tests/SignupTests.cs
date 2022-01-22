using System;
using System.Linq;
using System.Threading.Tasks;
using FluentEmail.Core;
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

namespace Wdpr_Groep_E.Tests
{
    public class SignupTests
    {
        [Fact]
        public void CreateSignupTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("CreateSignupTestDb").Options);

            var controller = new SignUpController(
                new Mock<IZmdhApi>().Object,
                new Mock<IFluentEmail>().Object,
                context,
                MockHelpers.MockUserManager<AppUser>().Object,
                MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            // Act
            var result = controller.CreateSignUp(new SignUp
            {
                FirstName = "Test",
                LastName = "User",
                Infix = "",
                PhoneNumber = "0612345678",
                Subject = "Test",
                Message = "Test",
                Email = "test@mail.nl",
                BirthDate = DateTime.Now.AddYears(-20),
                UserName = "TestUser"
            }, "test");

            var redirect = Assert.IsType<Task<IActionResult>>(result);

            // Assert
            Assert.Equal(1, context.SignUps.Count());
        }

        [Fact]
        public void CreateSignupWithChildTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("CreateSignupWithChildTestDb").Options);

            var controller = new SignUpController(
                new Mock<IZmdhApi>().Object,
                new Mock<IFluentEmail>().Object,
                context,
                MockHelpers.MockUserManager<AppUser>().Object,
                MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            // Act
            var result = controller.CreateSignUpWithChild(new SignUp
            {
                FirstName = "Test",
                LastName = "User",
                Infix = "",
                PhoneNumber = "0612345678",
                Subject = "Test",
                Message = "Test",
                Email = "test@mail.nl",
                BirthDate = DateTime.Now,
                UserName = "TestUser"
            }, new SignUpChild
            {
                ChildFirstName = "Test",
                ChildLastName = "User",
                ChildInfix = "",
                ChildBirthDate = DateTime.Now,
                ChildUserName = "TestChild"
            }, "test");

            var redirect = Assert.IsType<Task<IActionResult>>(result);

            // Assert
            Assert.Equal(1, context.SignUps.Count());
        }

        [Fact]
        public void AcceptSignupTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("AcceptSignupTestDb").Options);

            var controller = new SignUpController(
                new Mock<IZmdhApi>().Object,
                new Mock<IFluentEmail>().Object,
                context,
                MockHelpers.MockUserManager<AppUser>().Object,
                MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Add(new AppUser
            {
                FirstName = "Test",
                LastName = "User",
                Infix = "",
                PhoneNumber = "0612345678",
                Subject = "Test",
                Email = "test@mail.nl",
                BirthDate = DateTime.Now,
                UserName = "TestUser"
            });
            context.SaveChanges();

            // Assert
            Assert.Equal(1, context.Users.Count());
        }

        [Fact]
        public void AcceptSignupWithChildTest()
        {
            // Arrange
            var context = new WdprContext(new DbContextOptionsBuilder<WdprContext>().UseInMemoryDatabase("AcceptSignupWithChildTestDb").Options);

            var controller = new SignUpController(
                new Mock<IZmdhApi>().Object,
                new Mock<IFluentEmail>().Object,
                context,
                MockHelpers.MockUserManager<AppUser>().Object,
                MockHelpers.MockRoleManager<IdentityRole>().Object
            );

            context.Add(new AppUser
            {
                FirstName = "Test",
                LastName = "User",
                Infix = "",
                PhoneNumber = "0612345678",
                Subject = "Test",
                Email = "test@mail.nl",
                BirthDate = DateTime.Now,
                UserName = "TestUser"
            });
            context.SaveChanges();

            context.Add(new AppUser
            {
                FirstName = "Test",
                LastName = "User",
                Infix = "",
                BirthDate = DateTime.Now,
                UserName = "TestChild"
            });
            context.SaveChanges();

            // Assert
            Assert.Equal(2, context.Users.Count());
        }
    }
}
