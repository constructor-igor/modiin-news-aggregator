using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using ModiinNewsAggregator.Senders;

namespace ModiinNewsAggregator.Executers
{
    public class TaskExecuter : IAggregatorExecuter
    {
        #region IAggregatorExecuter
        public void Start()
        {
            var minute1 = new TimeSpan(hours: 0, minutes: 1, seconds: 0);
            var hour1 = new TimeSpan(hours: 1, minutes: 0, seconds: 0);

            var queue = new ConcurrentQueue<IMessage>();
            Task wheatherProducerTask = Task.Factory.StartNew(() =>
            {
                IProducer takeCurrentWeather = new LogDecoratorProducer(new UpdatesProducer(new PogodaModiinProducer()), includeEmptyMessage: false);
                while (true)
                {                    
                    queue.Enqueue(takeCurrentWeather.GetMessage());
                    Thread.Sleep(hour1);
                }
            });
            Task instagramProducerTask = Task.Factory.StartNew(() =>
            {
                IProducer instagramModiin = new LogDecoratorProducer(new UpdatesProducer(new InstagramProducer()), includeEmptyMessage: false);
                while (true)
                {                    
                    queue.Enqueue(instagramModiin.GetMessage());
                    Thread.Sleep(hour1);
                }
            });
            Task ModiinLjTask = Task.Factory.StartNew(() =>
            {
                IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer()), includeEmptyMessage: false);
                while (true)
                {
                    queue.Enqueue(takeLiveJournalUpdate.GetMessage());                    
                    Thread.Sleep(minute1);
                }
            });
            Task twitterSenderTask = Task.Factory.StartNew(() =>
            {
                ISender sender = new EmptyFilterSender(new LogDecoratorSender(new TwitterSender()));
                while (true)
                {                    
                    IMessage message;
                    if (queue.TryDequeue(out message))
                    {                        
                        sender.Send(message);
                    }
                }
            });
        }        
        #endregion
    }
}