using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using NUnit.Framework;

namespace AggregatorTests
{
    [TestFixture]
    public class GoogleTrafficParsingTests
    {
        [Test]
        public void Parse_CurrentTraffic28minutes()
        {
            //            const string xPath = @"/html/body/table/tbody/tr[547]/td[2]";
            //const string xPath = @"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]";
            //const string xPath = @"/html[1]/body/div/div/div[3]/div[6]/div/div[2]/div[3]/div/div/div/div/ol/li/div";
            const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[2]";
            string htmlContent = File.ReadAllText(@"..\..\Data\GoogleTrafficRehovotModiin.html");
            //
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode inCurrentTrafficNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);

            foreach (HtmlNode childNode in inCurrentTrafficNode.ChildNodes)
            {
                if (childNode.InnerHtml.Contains("In current"))
                {
                    Trace.WriteLine("XPath: ", childNode.XPath);
                }
            }

            Assert.IsNotNull(inCurrentTrafficNode);
            Assert.AreEqual("   In current traffic: 28 mins   ", inCurrentTrafficNode.InnerText);

        }
    }
}