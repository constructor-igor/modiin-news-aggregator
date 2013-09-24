using System;
using System.Threading;

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
                        sender.Send(message);
                        break;
                    case "weather":
                        IProducer modiinWeather = new PogodaModiinProducer();
                        Console.WriteLine(modiinWeather.GetContent());
                        break;
                    case "start":
                        const int SEC1 = 1000;
                        const int MIN1 = 60*SEC1;
                        const int HOUR1 = 60*MIN1;
                        var timer = new Timer(state =>
                        {
                            try
                            {
                                IProducer takeCurrentWeather = new PogodaModiinProducer();
                                ISender twitterSender = new TwitterSender();
                                twitterSender.Send(takeCurrentWeather.GetContent());
                            }
                            catch (TwitterErrorException e)
                            {
                                Console.WriteLine("Twit not sent, because {0}", e.Status.Content);
                            }
                        }, null, 0, HOUR1);
                        GC.KeepAlive(timer);
                        break;
                    default:
                        Console.WriteLine("unknown command: '{0}'", command);
                        break;                        
                }

            } while (!String.IsNullOrEmpty(command));
        }
    }

}
