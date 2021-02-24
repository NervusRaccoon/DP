using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Vaculator.Storage;

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

            string textKey = "TEXT-" + id;
            //TODO: сохранить в БД text по ключу textKey
            _storage.Store(textKey, text);

            string rankKey = "RANK-" + id;
            //TODO: посчитать rank и сохранить в БД по ключу rankKey
            _storage.Store(rankKey, GetRank(text).ToString());

            string similarityKey = "SIMILARITY-" + id;
            //TODO: посчитать similarity и сохранить в БД по ключу similarityKey
            double similarity = GetSimilarity(text, id);
            _storage.Store(similarityKey, similarity.ToString());

            return Redirect($"summary?id={id}");
        }

        private double GetRank(string text)
        {
            var count = text.Where(x => !Char.IsLetter(x)).Count();
            double rank = (double) count / text.Count();

            return rank;
        }
        private double GetSimilarity(string text, string id)
        {
            id = "TEXT-" + id;
            var keys = _storage.GetValues("TEXT-");
            foreach (var key in keys)
            {
                if(key != id && _storage.Load(key) == text)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
