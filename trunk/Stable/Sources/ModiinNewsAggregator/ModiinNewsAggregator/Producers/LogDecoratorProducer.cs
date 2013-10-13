using ModiinNewsAggregator.Interfaces;
using NLog;

namespace ModiinNewsAggregator.Producers
{
    public class LogDecoratorProducer : IProducer
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();

        private readonly IProducer actualProducer;
        private readonly bool includeEmptyMessage;

        public LogDecoratorProducer(IProducer actualProducer, bool includeEmptyMessage)
        {
            this.actualProducer = actualProducer;
            this.includeEmptyMessage = includeEmptyMessage;
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IMessage actualContent = actualProducer.GetMessage();
            if (!actualContent.Empty || includeEmptyMessage)
                m_logger.Trace("GetMessage(): {0}", actualContent);
            return actualContent;
        }
        #endregion
    }
}