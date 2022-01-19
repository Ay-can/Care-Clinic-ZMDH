using System;
using System.Diagnostics;
using System.IO;
using System.Net;
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

        public long LaadtijdHomepageTijdInitialize()
        {
            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            Stream data = client.OpenRead("http://aycan070-001-site1.ctempurl.com/");
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        [Fact]
        public void LaadtijdHomepageTijd()
        {
            long result = LaadtijdHomepageTijdInitialize();
            _output.WriteLine("Laadtijd homepage: " + result + "ms");
            Assert.True(result < 500);
        }
    }
}
