using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using ModiinNewsAggregator.Senders;

namespace ModiinNewsAggregator.Executers
{
    public class TrafficExecuter : IAggregatorExecuter
    {
        public void Start()
        {
            var minutes5 = new TimeSpan(hours: 0, minutes: 5, seconds: 0);
            const string uri = @"https://www.google.com/maps?sll=31.89527449987709%2C34.98711830006862&sspn=0.11295803168870334%2C0.27472621411297554&saddr=rehovot&t=m&output=classic&daddr=Modi%27in-Maccabim-Re%27ut%2C+Israel&dirflg=d";

            var queue = new ConcurrentQueue<IMessage>();
            Task weatherProducerTask = Task.Factory.StartNew(() =>
            {
                IProducer takeCurrentWeather = new LogDecoratorProducer(new UpdatesProducer(new GoogleTrafficProducer(new UriStreamCreator(uri))), includeEmptyMessage: false);
                while (true)
                {
                    queue.Enqueue(takeCurrentWeather.GetMessage());
                    Thread.Sleep(minutes5);
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
    }
}