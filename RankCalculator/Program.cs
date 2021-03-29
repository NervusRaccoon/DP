using NATS.Client;
using System;
using System.Text;
using System.Linq;
using Library;
using System.Threading.Tasks;

namespace RankCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            IStorage storage = new RedisStorage();
            
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
                PublishRankCalculate(id, rank);
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
        private static async void PublishRankCalculate(string id, double rank)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                Console.WriteLine($"1");
                string rank_str = id + " " + rank.ToString();
                byte[] data = Encoding.UTF8.GetBytes(rank_str);
                c.Publish("rank_calculator.processing.rank", data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
    }
}