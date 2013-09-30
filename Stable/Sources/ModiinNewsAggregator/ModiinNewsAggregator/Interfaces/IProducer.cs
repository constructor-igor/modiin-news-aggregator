namespace ModiinNewsAggregator.Interfaces
{
    public interface IProducer
    {
        IMessage GetMessage();
    }
}