using System;
using System.Threading;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using ModiinNewsAggregator.Senders;

namespace ModiinNewsAggregator
{
    class Program
    {
        static void Main()
        {
            string command;
            do
            {
                Console.Write("Enter command (Enter -> Exit):>");
                string input = Console.ReadLine();
                if (String.IsNullOrEmpty(input))
                    break;

                string[] words = input.Split(' ');
                command = words[0];

                switch (command)
                {
                    case "twit":
                        ISender sender = new TwitterSender();
                        string message = input.Remove(0, command.Length);
                        sender.Send(new MessageContainer(message));
                        break;
                    case "weather":
                        IProducer modiinWeather = new LogDecoratorProducer(new PogodaModiinProducer());
                        Console.WriteLine(modiinWeather.GetMessage());
                        break;
                    case "instagram":
                        IProducer instagramMedia = new InstagramProducer();
                        Console.WriteLine(instagramMedia.GetMessage());
                        break;
                    case "start":
                        Timer pogodaTimer = StartModiinPogodaTimer();
                        Timer liveJournlTimer = StartLiveJournalMonitoringTimer();

                        GC.KeepAlive(pogodaTimer);
                        GC.KeepAlive(liveJournlTimer);
                        break;
                    default:
                        Console.WriteLine("unknown command: '{0}'", command);
                        break;                        
                }

            } while (!String.IsNullOrEmpty(command));
        }

        private static Timer StartModiinPogodaTimer()
        {
            const int SEC1 = 1000;
            const int MIN1 = 60*SEC1;
            const int HOUR1 = 60*MIN1;
            const int PERIOD = HOUR1;
            var timer = new Timer(state =>
            {
                try
                {
                    IProducer takeCurrentWeather = new LogDecoratorProducer(new PogodaModiinProducer());
                    ISender twitterSender = new TwitterSender();
                    twitterSender.Send(takeCurrentWeather.GetMessage());

                    IProducer instagram = new InstagramProducer();
                    twitterSender.Send(instagram.GetMessage());
                }
                catch (TwitterErrorException e)
                {
                    Console.WriteLine("Twit not sent, because {0}", e.Status.Content);
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
            IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer()));
            var timer = new Timer(state =>
            {
                try
                {
                    IProducer producer = (IProducer) state;
                    ISender sender = new EmptyFilterSender(new LogDecoratorSender(new TwitterSender()));
                    sender.Send(producer.GetMessage());
                }
                catch (TwitterErrorException e)
                {
                    Console.WriteLine("Twit not sent, because {0}", e.Status.Content);
                }
            }, takeLiveJournalUpdate, 0, PERIOD);
            return timer;
        }
    }

}
