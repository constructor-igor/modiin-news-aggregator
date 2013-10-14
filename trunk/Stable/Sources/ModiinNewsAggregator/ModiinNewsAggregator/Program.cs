using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ModiinNewsAggregator.Executers;
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
                        var timerExecuter = new TimerExecuter();
                        timerExecuter.Start();
                        break;
                    case "start2":
                        IAggregatorExecuter taskExecuter = new TaskExecuter();
                        taskExecuter.Start();
                        break;
                    default:
                        Console.WriteLine("unknown command: '{0}'", command);
                        break;                        
                }

            } while (!String.IsNullOrEmpty(command));
            m_logger.Info("ModiinNewsAggregator finished");
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
