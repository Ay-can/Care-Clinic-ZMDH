using System.Net;
using System.IO;
using System.Diagnostics;
using Xunit;

namespace Webapp.Tests
{
    public class PerformanceTests{
        
        public long LaadtijdHomepageTijdInitialize(){ //Alleen nog de url aanpassen wanneer de website online staat op line 19.

            WebClient client = new WebClient();

            client.Headers.Add ("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

            Stopwatch timer = new Stopwatch();
            timer.Start();

            Stream data = client.OpenRead ("https://www.google.com");
            StreamReader reader = new StreamReader (data);
            string s = reader.ReadToEnd ();

            timer.Stop();
            return timer.ElapsedMilliseconds;
        }
        
        [Fact]
        public void LaadtijdHomepageTijd(){ //nog checken welke laadtijd in milliseconden een goede laadtijd is.

            long result = LaadtijdHomepageTijdInitialize();

            Assert.True(result < 1000);
        }

    }
}