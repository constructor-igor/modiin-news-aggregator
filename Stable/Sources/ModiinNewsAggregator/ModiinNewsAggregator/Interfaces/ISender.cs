namespace ModiinNewsAggregator.Interfaces
{
    public interface ISender
    {
        void Send(IMessage message);
    }
}