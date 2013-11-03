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
            var minutes15 = new TimeSpan(hours: 0, minutes: 15, seconds: 0);
            var minutes30 = new TimeSpan(hours: 0, minutes: 30, seconds: 0);
            var hour1 = new TimeSpan(hours: 1, minutes: 0, seconds: 0);

            var queue = new ConcurrentQueue<IMessage>();
            Task weatherProducerTask = Task.Factory.StartNew(() =>
            {
                IProducer takeCurrentWeather = new LogDecoratorProducer(new UpdatesProducer(new UpDownProducer(new PogodaModiinProducer())), includeEmptyMessage: false);
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
                    Thread.Sleep(minutes30);
                }
            });
            Task ModiinLjTask = Task.Factory.StartNew(() =>
            {
                IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer(@"http://modiin-ru.livejournal.com/data/atom")), includeEmptyMessage: false);
                while (true)
                {
                    queue.Enqueue(takeLiveJournalUpdate.GetMessage());                    
                    Thread.Sleep(minute1);
                }
            });
            Task nitsanimLjTask = Task.Factory.StartNew(() =>
            {
                IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer(@"http://nitsanim.livejournal.com/data/atom")), includeEmptyMessage: false);
                while (true)
                {
                    queue.Enqueue(takeLiveJournalUpdate.GetMessage());
                    Thread.Sleep(hour1);
                }
            });
            Task trafficTask = Task.Factory.StartNew(() =>
            {
                IProducer multiTrafficProducer = new GoogleMultiTrafficProducer("Modiin", new[] { "Jerusalem", "Tel-Aviv", "HaMada,Rehovot" });
                IProducer googleTrafficProducer = new LogDecoratorProducer(new UpdatesProducer(multiTrafficProducer), includeEmptyMessage: false);
                while (true)
                {
                    queue.Enqueue(googleTrafficProducer.GetMessage());
                    Thread.Sleep(minutes15);
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