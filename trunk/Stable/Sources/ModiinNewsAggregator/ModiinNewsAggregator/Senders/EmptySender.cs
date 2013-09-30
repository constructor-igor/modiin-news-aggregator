using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Senders
{
    public class EmptySender : ISender
    {
        #region ISender
        public void Send(string message)
        {
        }
        #endregion
    }
}