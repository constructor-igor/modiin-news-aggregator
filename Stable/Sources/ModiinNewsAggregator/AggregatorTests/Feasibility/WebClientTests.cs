using System;
using System.IO;
using System.Net;
using NUnit.Framework;

namespace AggregatorTests.Feasibility
{
    [TestFixture]
    public class WebClientTests
    {
        [Test]
        [Explicit]
        // http://msdn.microsoft.com/en-us/library/system.net.webclient.aspx
        public void LoadHtmlWebClient_PogodaModiin_Loaded()
        {
            const string uri = "http://pogoda.co.il/israel/modiin";
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            using (Stream data = client.OpenRead(uri))
            {
                using (var reader = new StreamReader(data))
                {
                    string s = reader.ReadToEnd();
                    //File.WriteAllText(@"d:\test.html", s, new UnicodeEncoding());
                    Console.WriteLine(s);
                }
            }
            Assert.Pass();
        }
    }
}