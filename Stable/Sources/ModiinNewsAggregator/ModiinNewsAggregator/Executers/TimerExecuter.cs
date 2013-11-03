using System;
using System.Threading;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using ModiinNewsAggregator.Senders;
using NLog;

namespace ModiinNewsAggregator.Executers
{
    public class TimerExecuter: IAggregatorExecuter
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();

        protected Timer pogodaTimer;
        protected Timer liveJournlTimer;

        #region IAggregatorExecuter
        public void Start()
        {
            pogodaTimer = StartModiinPogodaTimer();
            liveJournlTimer = StartLiveJournalMonitoringTimer();
        }
        #endregion
        private static Timer StartModiinPogodaTimer()
        {
            const int SEC1 = 1000;
            const int MIN1 = 60 * SEC1;
            const int HOUR1 = 60 * MIN1;
            const int PERIOD = HOUR1;
            var timer = new Timer(state =>
            {
                IMessage message = new MessageContainer();
                try
                {
                    IProducer takeCurrentWeather = new LogDecoratorProducer(new UpdatesProducer(new UpDownProducer(new PogodaModiinProducer())), includeEmptyMessage: false);
                    ISender masterSender = new EmptyFilterSender(new TwitterSender());
                    message = takeCurrentWeather.GetMessage();
                    masterSender.Send(message);

                    IProducer instagram = new LogDecoratorProducer(new UpdatesProducer(new InstagramProducer()), includeEmptyMessage: false);
                    masterSender.Send(instagram.GetMessage());
                }
                catch (TwitterErrorException e)
                {
                    m_logger.WarnException(String.Format("Twit '{0}' not sent, because {1}", message.Text, e.Status.Content), e);
                }
            }, null, 0, PERIOD);
            return timer;
        }
        private static Timer StartLiveJournalMonitoringTimer()
        {
            const int SEC1 = 1000;
            const int MIN1 = 60 * SEC1;
            const int HOUR1 = 60 * MIN1;
            const int PERIOD = MIN1;
            IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer(@"http://modiin-ru.livejournal.com/data/atom")), includeEmptyMessage: false);
            var timer = new Timer(state =>
            {
                IMessage message = new MessageContainer();
                try
                {
                    var producer = (IProducer)state;
                    ISender sender = new EmptyFilterSender(new LogDecoratorSender(new TwitterSender()));
                    message = producer.GetMessage();
                    sender.Send(message);
                }
                catch (TwitterErrorException e)
                {
                    m_logger.WarnException(String.Format("Twit '{0}' not sent, because {1}", message.Text, e.Status.Content), e);
                }
            }, takeLiveJournalUpdate, 0, PERIOD);
            return timer;
        }
    }
}
