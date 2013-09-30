using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using HtmlAgilityPack;
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
    public class LiveJournalAtomTests
    {
        [Test]
        public void ParseLiveJournalAtom()
        {
            const string postsXPath = @"/feed[1]";

            string atomContent = File.ReadAllText(@"..\..\Data\modiinLiveJournalAtom.xht");

            var htmlDoc = new HtmlDocument { OptionFixNestedTags = true };
            htmlDoc.Load(new StringReader(atomContent));
            HtmlNode postsNodesHtmlNode = htmlDoc.DocumentNode.SelectSingleNode(postsXPath);

            foreach (HtmlNode childNode in postsNodesHtmlNode.ChildNodes)
            {
                string innerText = childNode.InnerText;
                string name = childNode.Name;
                string xPath = childNode.XPath;
            }

            //
        }

        // http://modiin-ru.livejournal.com/data/rss
        [Test]
        public void ParseLiveJournalRss_Syndication()
        {
            const string rssUrl = @"..\..\Data\modiinLiveJournalRss.xht";

            using (TextReader atomReader = new StreamReader(rssUrl, Encoding.UTF8))
            {
                XmlReader xmlReader = XmlReader.Create(atomReader);
                var feed = SyndicationFeed.Load<SyndicationFeed>(xmlReader);

                Assert.AreEqual(25, feed.Items.Count());
            }
        }

        // http://modiin-ru.livejournal.com/data/atom
        [Test]
        public void ParseLiveJournalAtom_Syndication()
        {
            const string atomUrl = @"..\..\Data\modiinLiveJournalAtom.xht";

            using (TextReader atomReader = new StreamReader(atomUrl, Encoding.UTF8))
            {
                XmlReader xmlReader = XmlReader.Create(atomReader);
                var feed = SyndicationFeed.Load<SyndicationFeed>(xmlReader);

                Assert.AreEqual(25, feed.Items.Count());
                SyndicationItem lastItem = feed.Items.ElementAt(0);
                Assert.AreEqual(@"http://modiin-ru.livejournal.com/1033018.html", lastItem.Links[0].Uri.AbsoluteUri);
                Assert.AreEqual(@"где починить компьютер?", lastItem.Title.Text);
                Assert.AreEqual(635159015320000000, lastItem.PublishDate.Ticks);
                Assert.AreEqual("27/09/2013 5:58:52 PM +00:00", lastItem.PublishDate.ToString());
            }
        }
    }
}