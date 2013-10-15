using System;
using System.Text;
using ModiinNewsAggregator.Interfaces;

namespace ModiinNewsAggregator.Producers
{
    public class InstagramLiveJournalPageProducer : IProducer
    {
        private readonly IProducer instagProducer;
        public InstagramLiveJournalPageProducer(IProducer instagProducer)
        {
            this.instagProducer = instagProducer;
        }
        #region IProducer
        public IMessage GetMessage()
        {
            IMessage message = instagProducer.GetMessage();
            var instagramMediaDataMessage = message as InstagramMediaDataMessage;
            if (instagramMediaDataMessage == null)
                return new MessageContainer();

            var actualLiveJournalPostAsHtml = new StringBuilder();
            actualLiveJournalPostAsHtml.AppendLine();
            actualLiveJournalPostAsHtml.AppendLine("<lj-cut></lj-cut>");

            foreach (InstagramMedia media in instagramMediaDataMessage.MediaList)
            {
                actualLiveJournalPostAsHtml.AppendLine();
                actualLiveJournalPostAsHtml.AppendFormat("{0} by {1}", media.Caption, media.Author);
                if (!String.IsNullOrEmpty(media.Latitude))
                {
                    actualLiveJournalPostAsHtml
                        .AppendLine(String.Format(@" <a href=""https://www.google.com/maps?q={0},{1}&amp;num=1&amp;t=m&amp;z=18"" target=""_blank"">on Google Map</a>", media.Latitude, media.Longitude));
                }
                actualLiveJournalPostAsHtml.AppendLine(String
                        .Format(@"<img alt="""" border=""0"" src=""{0}"" title=""{1}"" />", media.FileUrl, media.Caption)
                        .Replace(@"\/", @"/"));
            }

            return new MessageContainer(actualLiveJournalPostAsHtml.ToString());
        }
        #endregion
    }
}