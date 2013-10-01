using System.Text;
using ModiinNewsAggregator.Interfaces;
using NLog;

namespace ModiinNewsAggregator.Senders
{
    public class LogDecoratorSender : ISender
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();
        readonly ISender actualSender;
        public LogDecoratorSender(ISender actualSender)
        {
            this.actualSender = actualSender;
        }
        #region ISender
        public void Send(string message)
        {
            m_logger.Trace("ISender.Send(): {0}", message);
            actualSender.Send(message);
        }
        #endregion
    }
}