using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    //http://potrebitel-il.livejournal.com/data/atom
    //http://modiin-ru.livejournal.com/data/atom"
    //http://nitsanim.livejournal.com/

    public class LiveJournalProducer : IProducer
    {
        //readonly List<string> uriList = new List<string>();
        private readonly string ljAtomLink;
        public LiveJournalProducer(string ljAtomLink)
        {
            this.ljAtomLink = ljAtomLink;
            //uriList.Add(@"http://potrebitel-il.livejournal.com/data/atom");
            //uriList.Add(@"http://modiin-ru.livejournal.com/data/atom");
            //uriList.Add(@"http://nitsanim.livejournal.com/data/atom");
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IMessage message = GetMessage(ljAtomLink);
            return message;
        }

        static IMessage GetMessage(string uri)
        {
            var client = new WebClient();
            using (Stream atomStream = client.OpenRead(uri))
            {
                using (TextReader atomReader = new StreamReader(atomStream, Encoding.UTF8))
                {
                    XmlReader xmlReader = XmlReader.Create(atomReader);
                    var feed = SyndicationFeed.Load<SyndicationFeed>(xmlReader);
                    if (!feed.Items.Any())
                    {
                        return new MessageContainer();
                    }
                    SyndicationItem lastPost = feed.Items.ElementAt(0);
                    string postUri = lastPost.Links[0].Uri.AbsoluteUri;
                    string postTitle = lastPost.Title.Text;
                    DateTimeOffset postTime = lastPost.PublishDate;
                    //string lastPostMessage = String.Format("Post '{0}' published {1}. Source: {2}", postTitle, postTime.LocalDateTime, postUri);

                    string lastPostMessage = String.Format("Новый пост от {0} (LJ) '{1}'. Источник: {2}", GetUserName(uri), postTitle, postUri);

                    //? how to parse uri

                    return new MessageContainer(lastPostMessage);
                }
            }
        }

        static string GetUserName(string path)
        {
            var uri = new Uri(path);
            string liveJournalUser = uri.Host.Split('.')[0];
            return liveJournalUser;
        }

        #endregion
    }
}