using NLog;

namespace ModiinNewsAggregator
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
        public string GetContent()
        {
            string actualContent = actualProducer.GetContent();
            m_logger.Trace("IProducer.GetContent(): {0}", actualContent);
            return actualContent;
        }
        #endregion
    }
}