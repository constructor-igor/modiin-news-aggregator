namespace ModiinNewsAggregator.Interfaces
{
    public interface IMessage
    {
        bool Empty { get; }
        string Text { get; }
    }
}