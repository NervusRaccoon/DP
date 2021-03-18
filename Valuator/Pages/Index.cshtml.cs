using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Valuator.Storage;
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

        public IActionResult OnPost(string text)
        {
            _logger.LogDebug(text);

            string id = Guid.NewGuid().ToString();

            string textKey = Constants.textPref + id;
            _storage.Store(textKey, text);

            string similarityKey = Constants.simPref + id;
            double similarity = GetSimilarity(text, id);
            _storage.Store(similarityKey, similarity.ToString());

            CreateRankCalculatorTask(id);

            return Redirect($"summary?id={id}");
        }
        private double GetSimilarity(string text, string id)
        {
            id = Constants.textPref + id;
            var keys = _storage.GetValues(Constants.textPref);
            foreach (var key in keys)
            {
                if(key != id && _storage.Load(key) == text)
                {
                    return 1;
                }
            }
            return 0;
        }
        private async void CreateRankCalculatorTask(string id)
        {
            CancellationTokenSource ct = new CancellationTokenSource();
            ConnectionFactory cf = new ConnectionFactory();
            using (IConnection c = cf.CreateConnection())
            {
                if (!ct.IsCancellationRequested)
                {
                    byte[] data = Encoding.UTF8.GetBytes(id);
                    c.Publish("valuator.processing.rank", data);
                    await Task.Delay(1000);
                }

                c.Drain();
                c.Close();
            }
        }
    }
}
