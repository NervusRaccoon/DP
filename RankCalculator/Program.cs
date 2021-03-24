using NATS.Client;
using System;
using System.Text;
using System.Linq;
using Library;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            IStorage storage = new RedisStorage();
            
            Console.WriteLine("Consumer started");
            ConnectionFactory cf = new ConnectionFactory();
            using IConnection c = cf.CreateConnection();
            var s = c.SubscribeAsync("valuator.processing.rank", "rank_calculator", (sender, args) =>
            {
                string id = Encoding.UTF8.GetString(args.Message.Data);
                string textKey = Constants.textPref + id;
                var text = storage.Load(textKey);
                string rankKey = Constants.rankPref + id;
                var rank = GetRank(text);
                storage.Store(rankKey, rank.ToString());
            });

            s.Start();

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();

            s.Unsubscribe();

            c.Drain();
            c.Close();
        }
        private static double GetRank(string text)
        {
            var count = text.Where(x => !Char.IsLetter(x)).Count();
            double rank = (double) count / text.Count();

            return rank;
        }
    }
}