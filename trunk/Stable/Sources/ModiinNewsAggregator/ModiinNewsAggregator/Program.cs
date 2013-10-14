using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ModiinNewsAggregator.Interfaces;
using ModiinNewsAggregator.Producers;
using ModiinNewsAggregator.Senders;
using NLog;

namespace ModiinNewsAggregator
{
    class Program
    {
        private static readonly Logger m_logger = LogManager.GetCurrentClassLogger();
        static void Main()
        {
            m_logger.Info("");
            m_logger.Info("ModiinNewsAggregator {0} started", Version);

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
                        IProducer modiinWeather = new LogDecoratorProducer(new PogodaModiinProducer(), includeEmptyMessage: false);
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
                    case "start2":
                        Start2();
                        break;
                    default:
                        Console.WriteLine("unknown command: '{0}'", command);
                        break;                        
                }

            } while (!String.IsNullOrEmpty(command));
            m_logger.Info("ModiinNewsAggregator finished");
        }

        private static Timer StartModiinPogodaTimer()
        {
            const int SEC1 = 1000;
            const int MIN1 = 60*SEC1;
            const int HOUR1 = 60*MIN1;
            const int PERIOD = HOUR1;
            var timer = new Timer(state =>
            {
                IMessage message = new MessageContainer();                
                try
                {
                    IProducer takeCurrentWeather = new LogDecoratorProducer(new UpdatesProducer(new PogodaModiinProducer()), includeEmptyMessage: false);
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
            IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer()), includeEmptyMessage: false);
            var timer = new Timer(state =>
            {
                IMessage message = new MessageContainer();
                try
                {
                    var producer = (IProducer) state;
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

        public static void Start2()
        {
            var queue = new ConcurrentQueue<IMessage>();
            Task wheatherProducerTask = Task.Factory.StartNew(() =>
            {
                IProducer takeCurrentWeather = new LogDecoratorProducer(new UpdatesProducer(new PogodaModiinProducer()), includeEmptyMessage: false);
                while (true)
                {                    
                    queue.Enqueue(takeCurrentWeather.GetMessage());
                    Thread.Sleep(new TimeSpan(hours: 1, minutes: 0, seconds: 0));
                }
            });
            Task instagramProducerTask = Task.Factory.StartNew(() =>
            {
                IProducer instagramModiin = new LogDecoratorProducer(new UpdatesProducer(new InstagramProducer()), includeEmptyMessage: false);
                while (true)
                {                    
                    queue.Enqueue(instagramModiin.GetMessage());
                    Thread.Sleep(new TimeSpan(hours: 1, minutes: 0, seconds: 0));
                }
            });
            Task ModiinLjTask = Task.Factory.StartNew(() =>
            {
                IProducer takeLiveJournalUpdate = new LogDecoratorProducer(new UpdatesProducer(new LiveJournalProducer()), includeEmptyMessage: false);
                while (true)
                {                    
                    queue.Enqueue(takeLiveJournalUpdate.GetMessage());
                    Thread.Sleep(new TimeSpan(hours: 0, minutes: 1, seconds: 0));
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
        public static string Version
        {
            get
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);
                return String.Format("{0}.{1}.{2}.{3}", fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart, fvi.ProductPrivatePart);
            }
        }
    }

}
