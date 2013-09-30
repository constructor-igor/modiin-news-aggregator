using System;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator
{
    public class MessageContainer : IMessage
    {
        public MessageContainer() : this(null)
        {
            
        }
        public MessageContainer(string text)
        {
            Text = text;
            Empty = String.IsNullOrEmpty(Text);
        }

        #region IMessage
        public bool Empty { get; private set; }
        public string Text { get; private set; }
        #endregion

        public override string ToString()
        {
            return Text;
        }
    }
}