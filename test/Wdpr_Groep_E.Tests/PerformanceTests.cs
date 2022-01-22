using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using Moq;
using Wdpr_Groep_E.Models;
using Wdpr_Groep_E.Services;
using Xunit;
using Xunit.Abstractions;

namespace Wdpr_Groep_E.Tests
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public PerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void LoadTimeHomepageTest()
        {
            WebClient client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            Stream Data = client.OpenRead("http://aycan070-001-site1.ctempurl.com/");
            StreamReader reader = new StreamReader(Data);
            string End = reader.ReadToEnd();
            Timer.Stop();
            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"Loadtime Home page: {Result}ms");
            Assert.True(Result < 1000);
        }

        [Fact]
        public void LoadInfoPageTest()
        {
            WebClient Client = new WebClient();
            Client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            Stream Data = Client.OpenRead("http://aycan070-001-site1.ctempurl.com/Info");
            StreamReader Reader = new StreamReader(Data);
            string End = Reader.ReadToEnd();
            Timer.Stop();
            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"Loadtime Info page :{Result}ms");
            Assert.True(Result < 1000);
        }
        [Fact]
        public void AboutUsPageTest()
        {
            WebClient Client = new WebClient();
            Client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            Stream Data = Client.OpenRead("http://aycan070-001-site1.ctempurl.com/About");
            StreamReader Reader = new StreamReader(Data);
            string End = Reader.ReadToEnd();
            Timer.Stop();
            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"Loadtime About page :{Result}ms");
            Assert.True(Result < 1000);
        }
        [Fact]
        public void OrthopedagoogPageTest()
        {
            WebClient Client = new WebClient();
            Client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            Stream Data = Client.OpenRead("http://aycan070-001-site1.ctempurl.com/About/Joseph_van_der_Vliet");
            StreamReader Reader = new StreamReader(Data);
            string End = Reader.ReadToEnd();
            Timer.Stop();
            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"Loadtime About page :{Result}ms");
            Assert.True(Result < 1000);
        }

        [Fact]
        public void ApiGetAllClientsPerformanceTest()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            ZmdhApi Api = new ZmdhApi(mockHttpClientFactory.Object);
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            var Send = Api.GetAllClients();
            Timer.Stop();

            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"Loadtime api call for all clients {Result}ms");
            Assert.True(Result < 1000);
        }
        [Fact]
        public void ApiGetAllReferralsPerformanceTest()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            Zorgdomein Zorgdomein = new Zorgdomein(mockHttpClientFactory.Object);
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            var Send = Zorgdomein.GetAllReferrals();
            Timer.Stop();
            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"Loadtime api call for all client referrals {Result}ms");
            Assert.True(Result < 1000);
        }
        [Fact]
        public void ApiPostClientPerformanceTest()
        {
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            ZmdhApi Api = new ZmdhApi(mockHttpClientFactory.Object);
            var generateId = Api.CreateClientId();
            Stopwatch Timer = new Stopwatch();
            Timer.Start();
            var Send = Api.PostClient(new Client() { clientid = Api.CreateClientId().Result, volledigenaam = "UnitTestUser" });
            Timer.Stop();
            var Result = Timer.ElapsedMilliseconds;
            _output.WriteLine($"New client added to api {Result} ms");
            Assert.True(Result < 500);
        }
    }
}
