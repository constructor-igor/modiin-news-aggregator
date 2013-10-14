using System;
using ModiinNewsAggregator.Interfaces;
using NLog;

namespace ModiinNewsAggregator.Senders
{
    public class LogDecoratorSender : ISender
    {
        static readonly Logger m_logger = LogManager.GetCurrentClassLogger();
        readonly ISender actualSender;
        public LogDecoratorSender(ISender actualSender)
        {
            this.actualSender = actualSender;
        }
        #region ISender
        public void Send(IMessage message)
        {
            m_logger.Trace("ISender.Send(): {0}", message.Text);
            try
            {
                actualSender.Send(message);
            }
            catch (TwitterErrorException e)
            {
                m_logger.ErrorException(String.Format("sending failed because {0}", e.Status.Content), e);
            }
            catch (Exception e)
            {
                m_logger.ErrorException(String.Format("sending failed because {0}", e.Message), e);
            }
        }
        #endregion
    }
}