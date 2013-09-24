using System;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using NUnit.Framework;
using WatiN.Core;

namespace AggregatorTests
{
    [TestFixture]
    public class FeasibilityTests
    {
        [Test]
        [Explicit]
        public void LoadHtmlWatiN_PogodaModiin_Grabbing()
        {
            using (var browser = new IE("http://pogoda.co.il/israel/modiin"))
            {
                Settings.WaitForCompleteTimeOut = 999999999; /*увеличиваем тайм-аут на всякий случай*/
                var doc = new HtmlDocument(); /* инсталляция объекта парсера HTML Agility Pack*/
                doc.LoadHtml(browser.Body.OuterHtml); /*помещаем в парсер полученный html с страницы yvison.kz*/
                Console.WriteLine(doc.ToString());
            }
            Assert.Pass();
        }

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

        [Test]
        public void ParseTemperature_PogodaModiin_28()
        {                                   
//            const string xPath = @"/html/body/table/tbody/tr[547]/td[2]";

            string htmlContent = File.ReadAllText(@"..\..\Data\PogodaModdin.html");

            var htmlDoc = new HtmlDocument {OptionFixNestedTags = true};
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode currentTemperature = htmlDoc.DocumentNode.SelectSingleNode(@"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]");
            Assert.AreEqual("+28&deg;", currentTemperature.InnerText);

//            HtmlNode nodeBody = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div/div[4]/div[2]/div[1]/strong");
//            foreach (HtmlNode node in nodeBody.ChildNodes)
//            {
//                Trace.WriteLine(String.Format("**** node name: {0}", node.Name));
//                Trace.WriteLine(String.Format("**** node Inner Text: {0}, Html: {1}, XPath: {2}", node.InnerText, node.InnerHtml, node.XPath));
//            }
//            HtmlNode node = doc.DocumentNode.SelectSingleNode(xPath);
//            Assert.AreEqual("28", node.OuterHtml);
        }
    }
}
