using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public class GoogleMultiTrafficProducer : IProducer
    {
        //
        // http://www.semclubhouse.com/special-characters-for-twitter/
        // → ← ↑ ↓
        // "https://www.google.com/maps?saddr=Modiin-Maccabim-Reut,Israel&daddr=Jerusalem,Israel"
        private readonly string baseAddress;
        private readonly List<IProducer> listOfTrafficProducers = new List<IProducer>();
        public GoogleMultiTrafficProducer(string baseAddress, IEnumerable<string> listOtherAddresses)
        {
            this.baseAddress = baseAddress;
            foreach (string otherAddress in listOtherAddresses)
            {
                string uri = String.Format("https://www.google.com/maps?saddr={0}&daddr={1}", baseAddress+",Israel", otherAddress+",Israel");
                var producer = new UpDownProducer(new GoogleTrafficProducer(baseAddress, otherAddress, new UriStreamCreator(uri)));
                listOfTrafficProducers.Add(producer);
            }
        }
        #region IProducer
        public IMessage GetMessage()
        {
            List<IMessage> messages = listOfTrafficProducers.Select(producer => producer.GetMessage()).ToList();
            return new GoogleMultiTrafficMessage(baseAddress, messages);
        }
        #endregion
    }

    public class GoogleMultiTrafficMessage : IMessage
    {
        private readonly String baseAddress;
       // private readonly List<GoogleTrafficMessage> messages;
        private readonly List<IMessage> messages;
        public GoogleMultiTrafficMessage(String baseAddress, List<IMessage> messages)
        {
            this.baseAddress = baseAddress;
            this.messages = messages;
            Text = GenerateText();
        }
        
        private string GenerateText()
        {
            var text = new StringBuilder();
            text.AppendFormat("#{0} To ", baseAddress);
            foreach (IMessage message in messages)
            {
                var actualMessage = message.ActualMessage as GoogleTrafficMessage;
                if (actualMessage != null)
                {
                    var upDownMessage = message as UpDownMessage;
                    string status = upDownMessage != null ? upDownMessage.StatusTo() : "";
                    text.AppendFormat("{0} {1}({2}), ", actualMessage.DestinationAddress, status, actualMessage.SuggestedTraffic[0].InCurrentTraffic.Trim());
                }
            }
            text.Remove(text.Length - 2, 2);
            return text.ToString();
        }

        #region IMessage
        public bool Empty { get { return String.IsNullOrEmpty(Text); } }
        public string Text { get; private set; }
        public IMessage ActualMessage { get { return this; } }
        #endregion

        public override string ToString()
        {
            return Text;
        }
    }
}