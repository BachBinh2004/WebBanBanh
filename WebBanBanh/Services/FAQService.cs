using Microsoft.EntityFrameworkCore;
using WebBanBanh.Models;
using System.Linq;

namespace WebBanBanh.Services
{
    public class FAQService
    {
        private readonly WebBanBanhContext _context;

        public FAQService(WebBanBanhContext context)
        {
            _context = context;
        }

        public string GetAnswer(string question)
        {
            var faqs = _context.FAQs.ToList();
            var bestMatch = faqs
                .OrderBy(f => LevenshteinDistance(f.Question.ToLower(), question.ToLower()))
                .FirstOrDefault();

            return (bestMatch != null && LevenshteinDistance(bestMatch.Question.ToLower(), question.ToLower()) <= 5)
                ? bestMatch.Answer
                : null;  // Trả về null để chatbot AI xử lý tiếp
        }

        private int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return string.IsNullOrEmpty(target) ? 0 : target.Length;
            if (string.IsNullOrEmpty(target))
                return source.Length;

            var d = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; d[i, 0] = i++) { }
            for (int j = 0; j <= target.Length; d[0, j] = j++) { }

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;
                    d[i, j] = System.Math.Min(
                        System.Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[source.Length, target.Length];
        }
    }
}
