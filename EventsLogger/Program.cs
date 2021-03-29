using System;
using NATS.Client;
using System.Text;

namespace EventsLogger
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();
            IAsyncSubscription s = c.SubscribeAsync("valuator.processing.similarity", (sender, args) =>
            {
                string[] data = Encoding.UTF8.GetString(args.Message.Data).Split(' ');
                Console.WriteLine($"Subject: {args.Message.Subject} ID: {data[0]} Similarity: {data[1]}");
            });

            s = c.SubscribeAsync("rank_calculator.processing.rank", (sender, args) =>
            {
                string[] data = Encoding.UTF8.GetString(args.Message.Data).Split(' ');
                Console.WriteLine($"Subject: {args.Message.Subject} ID: {data[0]} Rank: {data[1]}");
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
    }
}
