using System.Net;
using System.IO;
using System.Diagnostics;
using Xunit;
using System;
using Xunit.Abstractions;

namespace Webapp.Tests
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public PerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        // Alleen nog de url aanpassen wanneer de website online staat op line 19.
        public long LaadtijdHomepageTijdInitialize()
        {
            WebClient client = new WebClient();

            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            Stream data = client.OpenRead("https://www.google.com");
            StreamReader reader = new StreamReader(data);
            string s = reader.ReadToEnd();

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }

        // Nog checken welke laadtijd in milliseconden een goede laadtijd is.
        [Fact]
        public void LaadtijdHomepageTijd()
        {
            long result = LaadtijdHomepageTijdInitialize();
            _output.WriteLine("Laadtijd homepage: " + result + "ms");
            Assert.True(result < 1000);
        }
    }
}
