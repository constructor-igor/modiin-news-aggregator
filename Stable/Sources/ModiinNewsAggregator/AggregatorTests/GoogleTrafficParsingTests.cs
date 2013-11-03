using System.Diagnostics;
using System.IO;
using HtmlAgilityPack;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using Moq;
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
            const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[2]/span[1]";
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
                    Trace.WriteLine("Inner Text: ", childNode.InnerText);
                }
            }

            Assert.IsNotNull(inCurrentTrafficNode);
            Assert.AreEqual(" In current traffic: 28 mins ", inCurrentTrafficNode.InnerText);
        }
        [Test]
        public void Parse_StandardTraffic25minutes()
        {
            //            const string xPath = @"/html/body/table/tbody/tr[547]/td[2]";
            //const string xPath = @"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]";
            //const string xPath = @"/html[1]/body/div/div/div[3]/div[6]/div/div[2]/div[3]/div/div/div/div/ol/li/div";
            //const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[2]";
            const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[1]/span[2]";
            string htmlContent = File.ReadAllText(@"..\..\Data\GoogleTrafficRehovotModiin.html");
            //
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode standardTrafficNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);

            foreach (HtmlNode childNode in standardTrafficNode.ChildNodes)
            {
                if (childNode.InnerHtml.Contains("25 mins"))
                {
                    Trace.WriteLine("XPath: ", childNode.XPath);
                }
            }

            Assert.IsNotNull(standardTrafficNode);
            Assert.AreEqual("25 mins", standardTrafficNode.InnerText);
        }
    
        [Test]
        public void Parse_RouteNameRoute431()
        {
            //            const string xPath = @"/html/body/table/tbody/tr[547]/td[2]";
            //const string xPath = @"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]";
            //const string xPath = @"/html[1]/body/div/div/div[3]/div[6]/div/div[2]/div[3]/div/div/div/div/ol/li/div";
            //const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[2]";
            const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[3]";
            string htmlContent = File.ReadAllText(@"..\..\Data\GoogleTrafficRehovotModiin.html");
            //
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode standardTrafficNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);

            foreach (HtmlNode childNode in standardTrafficNode.ChildNodes)
            {
                if (childNode.InnerHtml.Contains("Route 431"))
                {
                    Trace.WriteLine("XPath: ", childNode.XPath);
                    Trace.WriteLine("InnerText: ", childNode.InnerText);
                }
            }

            Assert.IsNotNull(standardTrafficNode);
            Assert.AreEqual("Route 431", standardTrafficNode.InnerText);
        }
    }

    [TestFixture]
    public class GoogleTrafficProducerTests
    {
        [Test]
        public void GetMessage_2Traffics()
        {
            using (Stream htmlStream = File.OpenRead(@"..\..\Data\GoogleTrafficRehovotModiin.html"))
            {
                var mock = new Mock<IStreamCreator>();
                mock.Setup(foo => foo.CreateStream()).Returns(htmlStream);
               
                var producer = new GoogleTrafficProducer("Town1", "Town2", mock.Object);
                var message = producer.GetMessage() as GoogleTrafficMessage;
                const string expectedText = "Town1 - Town2 via Route 431: 28 mins (25 mins), Route 412 and Route 431: 38 mins (32 mins)";
                Assert.AreEqual(expectedText, message.Text);
                Assert.AreEqual(2, message.SuggestedTraffic.Count);
            }
        }
    }
}