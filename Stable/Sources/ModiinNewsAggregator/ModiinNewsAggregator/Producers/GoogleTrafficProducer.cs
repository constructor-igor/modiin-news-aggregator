using System;
using System.Collections.Generic;
using System.IO;
using HtmlAgilityPack;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public class GoogleTrafficProducer : IProducer
    {
        private readonly IStreamCreator streamCreator;
        public GoogleTrafficProducer(IStreamCreator streamCreator)
        {
            this.streamCreator = streamCreator;
        }
        #region IProducer
        public IMessage GetMessage()
        {
            const string xPath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[1]/div[1]/div[2]";

            using (Stream stream = streamCreator.CreateStream())
            {
                var htmlDoc = new HtmlDocument {OptionFixNestedTags = true};
                htmlDoc.Load(stream);

                HtmlNode inCurrentTrafficNode = htmlDoc.DocumentNode.SelectSingleNode(xPath);

                var message = new GoogleTrafficMessage();
                message.SuggestedTraffic.Add(new GoogleTrafficRoute { InCurrentTraffic = inCurrentTrafficNode.InnerText});
                message.GenerateText();
                return message;
            }
        }
        #endregion
    }

    public class GoogleTrafficMessage : IMessage
    {
        public IList<GoogleTrafficRoute> SuggestedTraffic { get; private set; }

        public GoogleTrafficMessage()
        {
            SuggestedTraffic = new List<GoogleTrafficRoute>();
        }

        public void GenerateText()
        {
            Text = SuggestedTraffic[0].InCurrentTraffic;
        }
        #region IMessage
        public bool Empty { get { return String.IsNullOrEmpty(Text); } }
        public string Text { get; private set; }
        #endregion
    }

    public class GoogleTrafficRoute
    {
        public string InCurrentTraffic { get; set; }
    }
}