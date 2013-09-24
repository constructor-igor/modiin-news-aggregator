using System.IO;
using System.Net;
using HtmlAgilityPack;
using System;

namespace ModiinNewsAggregator
{
    public class PogodaModiinProducer : IProducer
    {
        #region IProducer
        public string GetContent()
        {
            string htmlContent = GetPogodaModiinPage();

            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode currentTemperature = htmlDoc.DocumentNode.SelectSingleNode(@"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]");
            string innerText = currentTemperature.InnerText;
            string[] contentStrings = innerText.Split('&');
            return String.Format("#Modiin - now {0}\u00B0C. Source: http://pogoda.co.il/israel/modiin", contentStrings[0]);
        }
        #endregion
        private string GetPogodaModiinPage()
        {
            const string uri = "http://pogoda.co.il/israel/modiin";
            var client = new WebClient();
            client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
            using (Stream data = client.OpenRead(uri))
            {
                using (var reader = new StreamReader(data))
                {
                    string s = reader.ReadToEnd();
                    return s;
                }
            }
        }
    }
}