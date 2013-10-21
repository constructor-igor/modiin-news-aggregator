using System.IO;
using System.Net;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    public class GoogleMapTests
    {
        [Test]
        [Explicit]
        public void CurrentTrafficGrabber()
        {
            const string uri = @"https://www.google.com/maps?sll=31.89527449987709%2C34.98711830006862&sspn=0.11295803168870334%2C0.27472621411297554&saddr=rehovot&t=m&output=classic&daddr=Modi%27in-Maccabim-Re%27ut%2C+Israel&dirflg=d";
            using (var client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                using (Stream data = client.OpenRead(uri))
                {
                    using (var reader = new StreamReader(data))
                    {
                        string s = reader.ReadToEnd();
                        File.WriteAllText(@"d:\mapSample.html", s);
                    }
                }
            }
        }

        ////*[@id="altroute_0"]
        /// //*[@id="altroute_0"]//*[@id="altroute_0"]
        /// //*[@id="altroute_0"]
        /// /html/body/div/div/div[3]/div[6]/div/div[2]/div[3]/div/div/div/div/ol/li[2]/div/div[2]/span
        /// /html/body/div/div/div[3]/div[6]/div/div[2]/div[3]/div/div/div/div/ol/li/div
    }
}