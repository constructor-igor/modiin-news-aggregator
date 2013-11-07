using System;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator
{
    public class MessageContainer : IMessage
    {
        private readonly IMessage m_actualMessage;
        public MessageContainer() : this(null)
        {
            
        }
        public MessageContainer(string text): this(text, null)
        {
        }
        public MessageContainer(string text, IMessage actualMessage)
        {
            Text = text;
            Empty = String.IsNullOrEmpty(Text);
            m_actualMessage = actualMessage;
        }

        #region IMessage
        public bool Empty { get; private set; }
        public string Text { get; private set; }
        public IMessage ActualMessage { get { return m_actualMessage ?? this; } }
        #endregion

        public override string ToString()
        {
            return Text;
        }
    }
}