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
    public class LiveJournalSyndicationTests
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