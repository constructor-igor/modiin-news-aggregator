using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public class PogodaModiinProducer : IProducer
    {
        #region IProducer
        public IMessage GetMessage()
        {
            string htmlContent = GetPogodaModiinPage();

            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode currentTemperature = htmlDoc.DocumentNode.SelectSingleNode(@"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]");
            string currentTemperatureText = currentTemperature.InnerText;
            string[] contentStrings = currentTemperatureText.Split('&');
            
            HtmlNode dataUpdated = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div/div[4]/div[2]/div[1]/p[2]/b[1]");
            string dataUpdatedText = dataUpdated.InnerText;

            return new MessageContainer(String.Format("#Modiin - now {0}\u00B0C. Source: http://pogoda.co.il/israel/modiin, updated {1}", contentStrings[0], dataUpdatedText));
        }
        #endregion
        string GetPogodaModiinPage()
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