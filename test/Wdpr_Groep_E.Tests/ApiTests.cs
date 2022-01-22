using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Wdpr_Groep_E.Controllers;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;
using Xunit;
using Xunit.Abstractions;

namespace Wdpr_Groep_E.Tests
{
    public class ApiTests
    {

        [Fact]
        public void GetAllReferralsTest()
        {
            //Arrange
            var mockApi = new Mock<IZorgdomein>();
            mockApi.Setup(s => s.GetAllReferrals().Result);
            var Controller = new ReferralController(mockApi.Object);
            //Act
            var Result = Controller.Index(null,0,0,null);
            //Assert
            var viewResult = Assert.IsType<ViewResult>(Result);
            var Model = Assert.IsAssignableFrom<IEnumerable<ReferralOverview>>(viewResult.ViewData.Model);
        }
        //[Fact]
        // public void GetReferralObjectTest()
        // {
        //     //Arrange
        //     var mockApi = new Mock<IZorgdomein>();
        //     mockApi.Setup(s => s.GetAllReferrals().Result);
        //     var Controller = new ReferralController(mockApi.Object);
        //     //Act
        //     var Result = Controller.Index(null,0,0,null);
        //     //Assert
        //     var viewResult = Assert.IsType<ViewResult>(Result);
        //     var Model = Assert.IsAssignableFrom<IEnumerable<ReferralOverview>>(viewResult.ViewData.Model);

        // //    mockApi.Verify(s => s.GetAllReferrals(),Times.Once());
        //     Assert.Equal(Model.First().bsn,Model.First().bsn);


        //     // //Arrange
        //     // var mockApi = new Mock<IZorgdomein>();
        //     // mockApi.Setup(s => s.GetReferralObject(It.IsAny<string>(),It.IsAny<string>()));
        //     // var Controller = new ReferralController(mockApi.Object);
        //     // //Act
        //     // var Result = Controller.IndividualReferral("003550540","01/02/2014");
        //     // //Assert
        //     // var viewResult = Assert.IsType<ViewResult>(Result);
        //     // var Model = Assert.IsAssignableFrom<Referral>(viewResult.ViewData.Model);
         //}
         [Fact]
         public void GenerateKeyHeaderTest()
         {
             //Arrange
             var mockHttpClientFactory = new Mock<IHttpClientFactory>();
             Zorgdomein z = new Zorgdomein(mockHttpClientFactory.Object);
            //Act
             var generateKeyHeader = z.KeyHeader();
            //Assert
            Assert.NotNull(generateKeyHeader);
         }

    }
}
