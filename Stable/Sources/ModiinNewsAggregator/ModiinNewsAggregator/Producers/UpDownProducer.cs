using System;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
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
                    updatedMessage = new MessageContainer("↑ " + actualMessage.Text); 
                if (lastValue > newValue)
                    updatedMessage = new MessageContainer("↓ " + actualMessage.Text); 
            }
            finally
            {
                lastValue = newValue;
            }
            return updatedMessage;
        }
    }
}