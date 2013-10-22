using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using ModiinNewsAggregator.Senders;
using System.Collections.Generic;

namespace ModiinNewsAggregator.Executers
{
    public class TrafficExecuter : IAggregatorExecuter
    {
        public void Start()
        {
            var minutes5 = new TimeSpan(hours: 0, minutes: 5, seconds: 0);
            //const string uri_Rehovot_Modiin = @"https://www.google.com/maps?sll=31.89527449987709%2C34.98711830006862&sspn=0.11295803168870334%2C0.27472621411297554&saddr=rehovot&t=m&output=classic&daddr=Modi%27in-Maccabim-Re%27ut%2C+Israel&dirflg=d";
            const string uri_HaMadaRehovot_Modiin = @"https://www.google.com/maps?sll=31.89527449987709%2C34.98711830006862&sspn=0.11295803168870334%2C0.27472621411297554&saddr=HaMada,+Rehovot&t=m&output=classic&daddr=Modi%27in-Maccabim-Re%27ut%2C+Israel&dirflg=d";
            const string uri_Modiin_TelAviv = @"https://www.google.com/maps?saddr=Modi%27in-Maccabim-Re%27ut,+Israel&daddr=Tel-Aviv&hl=en&sll=31.97837,34.893003&sspn=0.195119,0.41851&geocode=FVub5gEdXTcWAik1mny6xM0CFTEDMhPlv2xfrw%3BFW5K6QEd26oSAikffDsZpkwdFTGQP5bAonL7wQ&mra=ls&t=m&z=12";
            const string uri_Modiin_Jerusalem = @"https://www.google.com/maps?saddr=Modi'in-Maccabim-Re'ut,+Israel&daddr=Jerusalem&hl=en&ll=31.833233,35.111961&spn=0.195427,0.41851&sll=31.97837,34.893003&sspn=0.195119,0.41851&geocode=FVub5gEdXTcWAik1mny6xM0CFTEDMhPlv2xfrw%3BFf--5AEdjlEZAilL_ME01tcCFTHL4W5FPmJv2Q&mra=ls&t=m&z=12";

            var uriList = new List<Tuple<string, string>>();
            //uriList.Add(new Tuple<string, string>("Rehovot - Modiin", uri_Rehovot_Modiin));
            uriList.Add(new Tuple<string, string>("HaMada, Rehovot - Modiin", uri_HaMadaRehovot_Modiin));   // http://goo.gl/lniEYK
            uriList.Add(new Tuple<string, string>("Modiin - Tel-Aviv", uri_Modiin_TelAviv));
            uriList.Add(new Tuple<string, string>("Modiin - Jerusalem", uri_Modiin_Jerusalem));

            var queue = new ConcurrentQueue<IMessage>();

            Task task = Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    foreach (Tuple<string, string> uriItem in uriList)
                    {
                        string direction = uriItem.Item1;
                        string uri = uriItem.Item2;
                        IProducer googleTrafficProducer = new LogDecoratorProducer(new UpdatesProducer(new GoogleTrafficProducer(direction, new UriStreamCreator(uri))), includeEmptyMessage: false);
                        queue.Enqueue(googleTrafficProducer.GetMessage());
                    }
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