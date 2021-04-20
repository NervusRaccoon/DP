using System;
using Library;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using NATS.Client;
using System.Text;

namespace Valuator.Pages
{
    public class IndexModel : PageModel
    {       
        private readonly ILogger<IndexModel> _logger;
        private readonly IStorage _storage;

        public IndexModel(ILogger<IndexModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public void OnGet()
        {

        }

        public IActionResult OnPost(string text, string shardKey)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();
            
            _storage.StoreShardKey(id, shardKey); 

            string textKey = Constants.textPref + id;
            _storage.Store(textKey, shardKey, text);

            string similarityKey = Constants.simPref + id;
            double similarity = GetSimilarity(text, id);
            _storage.Store(similarityKey, shardKey, similarity.ToString());
            _storage.StoreToSet(Constants.setKey, shardKey, text);

            _logger.LogDebug($"LOOKUP: {id}, {shardKey}");

            PublishSimilarityCalculate(id, similarity);
            CreateRankCalculatorTask(id);

            return Redirect($"summary?id={id}");
        }
        private double GetSimilarity(string text, string id)
        {
            return _storage.IsValueExist(Constants.setKey, text) ? 1 : 0;
        }
        private async void PublishSimilarityCalculate(string id, double similarity)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                string similarity_str = id + " " + similarity.ToString();
                byte[] data = Encoding.UTF8.GetBytes(similarity_str);
                c.Publish("valuator.processing.similarity", data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
        private async void CreateRankCalculatorTask(string id)
        {
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                byte[] data = Encoding.UTF8.GetBytes(id);
                c.Publish("valuator.processing.rank", data);
                await Task.Delay(1000);

                c.Drain();
                c.Close();
            }
        }
    }
}
