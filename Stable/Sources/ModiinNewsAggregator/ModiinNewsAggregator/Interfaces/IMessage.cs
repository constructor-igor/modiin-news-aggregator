namespace ModiinNewsAggregator.Interfaces
{
    public interface IMessage
    {
        bool Empty { get; }
        string Text { get; }
        IMessage ActualMessage { get; }
    }

    public interface IValueMessage
    {
        double Value { get; }
    }

}