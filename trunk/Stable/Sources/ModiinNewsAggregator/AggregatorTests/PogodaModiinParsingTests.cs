using System;
using System.IO;
using HtmlAgilityPack;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using Moq;
using NUnit.Framework;

namespace AggregatorTests
{
    [TestFixture]
    public class PogodaModiinParsingTests
    {
        [Test]
        public void ParsePogodaModiin_Temperature_28()
        {
            //            const string xPath = @"/html/body/table/tbody/tr[547]/td[2]";
            const string xPath = @"/html[1]/body[1]/div[1]/div[4]/div[2]/div[1]/strong[1]";

            string htmlContent = File.ReadAllText(@"..\..\Data\PogodaModdin.html");

            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

            HtmlNode currentTemperature = htmlDoc.DocumentNode.SelectSingleNode(xPath);
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

        [Test]
        public void ParsePogodaModiin_Updated_1150()
        {
            const string xPath = @"/html/body/div/div[4]/div[2]/div[1]/p[2]/b[1]";

            string htmlContent = File.ReadAllText(@"..\..\Data\PogodaModdin.html");
            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(htmlContent));

//            HtmlNode nodeBody = htmlDoc.DocumentNode.SelectSingleNode(@"/html/body/div/div[4]/div[2]/div[1]/p[2]/b[1]");
//            foreach (HtmlNode node in nodeBody.ChildNodes)
//            {
//                System.Diagnostics.Trace.WriteLine(String.Format("**** node name: {0}", node.Name));
//                System.Diagnostics.Trace.WriteLine(String.Format("**** node Inner Text: {0}, Html: {1}, XPath: {2}", node.InnerText, node.InnerHtml, node.XPath));
//            }
            HtmlNode updateNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);
            Assert.AreEqual("11:50", updateNode.InnerText);
        }
    }

    [TestFixture]
    public class TimesConvertationTests
    {
        [Test]
        public void ConvertToJerusalemTime()
        {
            var universalTime = new DateTimeOffset(2013, 10, 01, 18, 30, 0, TimeSpan.FromHours(5));
            Assert.AreEqual("01/10/2013 6:30:00 PM", universalTime.DateTime.ToString());

            DateTimeOffset postTime = TimeZoneInfo.ConvertTime(universalTime, TimeZoneInfo.Local);
            Assert.AreEqual("01/10/2013 4:30:00 PM", postTime.DateTime.ToString());
        }
    }

    [TestFixture]
    public class PogodaUpDownProducerTests
    {
        [Test]
        public void TestUp()
        {
            var mock = new Mock<IProducer>();
            PogodaMessage pogodaMessage = new PogodaMessage("+18", "11:50");
            mock.Setup(foo => foo.GetMessage()).Returns(() => pogodaMessage);

            var producer = new PogodaUpDownProducer(mock.Object);
            IMessage firstMessage = producer.GetMessage();
            Assert.AreEqual("#Modiin - now 18°C. Source: http://pogoda.co.il/israel/modiin, updated 11:50", firstMessage.Text);

            pogodaMessage = new PogodaMessage("+19", "12:10");
            IMessage secondMessage = producer.GetMessage();
            Assert.AreEqual("↑ #Modiin - now 19°C. Source: http://pogoda.co.il/israel/modiin, updated 12:10", secondMessage.Text);
        }

        [Test]
        public void TestDown()
        {
            var mock = new Mock<IProducer>();
            PogodaMessage pogodaMessage = new PogodaMessage("+18", "11:50");
            mock.Setup(foo => foo.GetMessage()).Returns(() => pogodaMessage);

            var producer = new PogodaUpDownProducer(mock.Object);
            IMessage firstMessage = producer.GetMessage();
            Assert.AreEqual("#Modiin - now 18°C. Source: http://pogoda.co.il/israel/modiin, updated 11:50", firstMessage.Text);

            pogodaMessage = new PogodaMessage("+17", "12:10");
            IMessage secondMessage = producer.GetMessage();
            Assert.AreEqual("↓ #Modiin - now 17°C. Source: http://pogoda.co.il/israel/modiin, updated 12:10", secondMessage.Text);
        }
    }
}