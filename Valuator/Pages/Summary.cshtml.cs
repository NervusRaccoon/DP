using System;
using Library;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Valuator.Pages
{
    public class SummaryModel : PageModel
    {
        private readonly ILogger<SummaryModel> _logger;
        private IStorage _storage;

        public SummaryModel(ILogger<SummaryModel> logger, IStorage storage)
        {
            _logger = logger;
            _storage = storage;
        }

        public double Rank { get; set; }
        public double Similarity { get; set; }
        public bool IsRankEmpty { get; set; }

        public void OnGet(string id)
        {
            _logger.LogDebug(id);

            Similarity = Convert.ToDouble(_storage.Load(Constants.simPref + id.ToString()));
            Rank = Convert.ToDouble(_storage.Load(Constants.rankPref + id.ToString()));
        }
    }
}
