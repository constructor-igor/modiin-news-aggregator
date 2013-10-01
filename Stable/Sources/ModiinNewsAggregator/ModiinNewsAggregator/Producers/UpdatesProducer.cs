using System;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public class UpdatesProducer : IProducer
    {
        readonly IProducer actualProducer;
        private string keyOfLastMessage = String.Empty;
        public UpdatesProducer(IProducer actualProducer)
        {
            this.actualProducer = actualProducer;
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IMessage actualMessage = actualProducer.GetMessage();
            string actualKey = actualMessage.ToString();
            if (actualKey == keyOfLastMessage)
                return new MessageContainer();

            keyOfLastMessage = actualKey;
            return actualMessage;
        }
        #endregion
    }
}