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
        readonly List<string> uriList = new List<string>();
        public LiveJournalProducer()
        {
            //uriList.Add(@"http://potrebitel-il.livejournal.com/data/atom");
            uriList.Add(@"http://modiin-ru.livejournal.com/data/atom");
            uriList.Add(@"http://nitsanim.livejournal.com/");
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IMessage message = GetMessage(uriList[0]);
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
                    string lastPostMessage = String.Format("Новый пост от modiin_ru (LJ) '{0}'. Источник: {1}", postTitle, postUri);

                    return new MessageContainer(lastPostMessage);
                }
            }
        }

        #endregion
    }
}