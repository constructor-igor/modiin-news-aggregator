using System;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public enum UpDownStatus { Undefined, Up,  Down}
    // → ← ↑ ↓
    public class UpDownProducer : IProducer
    {
        private const double VALUE_TOLERANCE = 0.00001;
        private readonly IProducer actualProducer;
        private double lastValue = double.NaN;
        public UpDownProducer(IProducer actualProducer)
        {
            this.actualProducer = actualProducer;
        }
        public IMessage GetMessage()
        {            
            IMessage actualMessage = actualProducer.GetMessage();
            var valueMessage = actualMessage as IValueMessage;
            if (valueMessage == null)
                return actualMessage;
            double actualMessageValue = valueMessage.Value;
            if (double.IsNaN(actualMessageValue))
                return actualMessage;
            if (double.IsNaN(lastValue))
            {
                lastValue = actualMessageValue;
            }
            if (Math.Abs(lastValue - actualMessageValue) < VALUE_TOLERANCE)
                return actualMessage;

            IMessage updatedMessage = actualMessage;
            double newValue = actualMessageValue;
            try
            {
                if (lastValue < newValue)
                {
                    updatedMessage = new UpDownMessage(actualMessage, UpDownStatus.Up);
                }
                if (lastValue > newValue)
                {
                    updatedMessage = new UpDownMessage(actualMessage, UpDownStatus.Down);
                }
            }
            finally
            {
                lastValue = newValue;
            }
            return updatedMessage;
        }
    }

    public class UpDownMessage : IMessage
    {
        private readonly UpDownStatus m_status;
        public bool Empty { get { return String.IsNullOrEmpty(Text); }}
        public string Text { get { return String.Format("{0} {1}", StatusTo(), ActualMessage.Text); } }
        public IMessage ActualMessage { get; private set; }

        public UpDownMessage(IMessage actualMessage, UpDownStatus status)
        {
            ActualMessage = actualMessage;
            m_status = status;
        }

        public string StatusTo()
        {
            switch (m_status)
            {
                case UpDownStatus.Down:
                    return "↓";
                case UpDownStatus.Up:
                    return "↑";
                default:
                    return "";
            }
        }
    }
}