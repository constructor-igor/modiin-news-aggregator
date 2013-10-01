using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Senders
{
    public class EmptyFilterSender : ISender
    {
        readonly ISender actualSender;
        public EmptyFilterSender(ISender actualSender)
        {
            this.actualSender = actualSender;
        }
        #region ISender
        public void Send(IMessage message)
        {
            if (!message.Empty)
                actualSender.Send(message);
        }
        #endregion
    }
}