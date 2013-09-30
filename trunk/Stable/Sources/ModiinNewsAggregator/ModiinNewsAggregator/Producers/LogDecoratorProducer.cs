using ModiinNewsAggregator.Interfaces;
using NLog;

namespace ModiinNewsAggregator.Producers
{
    public class LogDecoratorProducer : IProducer
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();

        private readonly IProducer actualProducer;

        public LogDecoratorProducer(IProducer actualProducer)
        {
            this.actualProducer = actualProducer;
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IMessage actualContent = actualProducer.GetMessage();
            m_logger.Trace("IProducer.GetMessage(): {0}", actualContent);
            return actualContent;
        }
        #endregion
    }
}