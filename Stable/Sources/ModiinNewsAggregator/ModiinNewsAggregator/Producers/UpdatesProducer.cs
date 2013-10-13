using System;
using ModiinNewsAggregator.Interfaces;
using NLog;

namespace ModiinNewsAggregator.Producers
{
    public class UpdatesProducer : IProducer
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();
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
            {
                m_logger.Warn("Generated message with not updated key: {0}", actualKey);
                return new MessageContainer();
            }

            keyOfLastMessage = actualKey;
            return actualMessage;
        }
        #endregion
    }
}