using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HtmlAgilityPack;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public class GoogleTrafficProducer : IProducer
    {
        readonly string sourceAddress;
        readonly string destinationAddress;
        readonly IStreamCreator streamCreator;
        public GoogleTrafficProducer(string sourceAddress, string destinationAddress, IStreamCreator streamCreator)
        {
            this.sourceAddress = sourceAddress;
            this.destinationAddress = destinationAddress;            
            this.streamCreator = streamCreator;
        }
        #region IProducer
        public IMessage GetMessage()
        {
            const string routePath = @"/html[1]/body[1]/div[4]/div[6]/div[1]/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/ol[1]/li[{0}]";

            using (Stream stream = streamCreator.CreateStream())
            {
                var htmlDoc = new HtmlDocument {OptionFixNestedTags = true};
                htmlDoc.Load(stream);

                var message = new GoogleTrafficMessage(sourceAddress, destinationAddress);
                HtmlNode routeNode = null;
                int routeIndex = 1;
                while ((routeNode = htmlDoc.DocumentNode.SelectSingleNode(String.Format(routePath, routeIndex++))) != null)
                {
                    HtmlNodeCollection routeNameNode = routeNode.SelectNodes("div[1]/div[3]");
                    HtmlNodeCollection standardTrafficNode = routeNode.SelectNodes("div[1]/div[1]/span[2]");
                    HtmlNodeCollection currentTrafficNode = routeNode.SelectNodes("div[1]/div[2]/span[1]");

                    string inCurrentTraffic = currentTrafficNode[0].InnerText.Remove(0, "In current traffic: ".Length);

                    var googleTrafficRoute = new GoogleTrafficRoute
                    {
                        RouteName = routeNameNode[0].InnerText,
                        StandardTraffic = standardTrafficNode[0].InnerText,
                        InCurrentTraffic = inCurrentTraffic
                    };
                    message.SuggestedTraffic.Add(googleTrafficRoute);
                }
                message.GenerateText();
                return message;
            }
        }
        #endregion
    }

    public class GoogleTrafficMessage : IMessage
    {
        public string SourceAddress { get; private set; }
        public string DestinationAddress { get; private set; }
        public string DirectionCaption { get; private set; }
        public IList<GoogleTrafficRoute> SuggestedTraffic { get; private set; }

        public GoogleTrafficMessage(string sourceAddress, string destinationAddress)
        {
            SourceAddress = sourceAddress;
            DestinationAddress = destinationAddress;
            DirectionCaption = String.Format("{0} - {1}", sourceAddress, destinationAddress);
            SuggestedTraffic = new List<GoogleTrafficRoute>();
        }

        public void GenerateText()
        {
            Text = String.Format("{0} via ", DirectionCaption);
            if (SuggestedTraffic.Count == 0)
            {
                return;
            }

            var text = new StringBuilder();
            foreach (GoogleTrafficRoute googleTrafficRoute in SuggestedTraffic)
            {
                text.AppendFormat("{0}: {1} ({2}), ", googleTrafficRoute.RouteName, googleTrafficRoute.InCurrentTraffic.Trim(), googleTrafficRoute.StandardTraffic);
            }

            text.Remove(text.Length-2, 2);
            Text += text.ToString();
        }
        #region IMessage
        public bool Empty { get { return String.IsNullOrEmpty(Text); } }
        public string Text { get; private set; }
        #endregion

        public override string ToString()
        {
            return Text;
        }
    }

    public class GoogleTrafficRoute
    {
        public string RouteName { get; set; }
        public string StandardTraffic { get; set; }
        public string InCurrentTraffic { get; set; }
    }
}