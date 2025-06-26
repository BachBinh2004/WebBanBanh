
using Microsoft.AspNetCore.Mvc;
using WebBanBanh.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WebBanBanh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatbotController : ControllerBase
    {
        private readonly WebBanBanhContext _dbContext;

        public ChatbotController(WebBanBanhContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("ask")]
        public async Task<ActionResult> AskQuestionAsync([FromBody] ChatbotRequest request)
        {
            if (string.IsNullOrEmpty(request.Question))
                return BadRequest("Vui lòng nhập câu hỏi!");

            // KIỂM TRA FAQs TRƯỚC
            var faq = await _dbContext.FAQs
                .OrderByDescending(f => f.Id) // Ưu tiên câu hỏi mới nhất
                .FirstOrDefaultAsync(f => request.Question.Contains(f.Question));

            if (faq != null)
                return Ok(new { reply = faq.Answer });

            // NẾU KHÔNG CÓ THÔNG TIN TRONG FAQs, KIỂM TRA BÁNH
            var banhResponse = await GetBanhInfoAsync(request.Question);
            if (banhResponse != null)
            {
                return Ok(new { reply = banhResponse });
            }

            return Ok(new { reply = "Xin lỗi, tôi chưa có câu trả lời cho câu hỏi này." });
        }

        // 📌 Truy xuất thông tin bánh dựa trên nội dung câu hỏi
        private async Task<string> GetBanhInfoAsync(string question)
        {
            var danhSachBanh = await _dbContext.Banhs
                .Where(b => !b.IsHidden) // Lọc bánh ẩn
                .ToListAsync();

            // Tìm bánh có tên gần giống nhất với câu hỏi
            var banhTimThay = danhSachBanh
                .OrderBy(b => LevenshteinDistance(b.TenBanh.ToLower(), question.ToLower()))
                .FirstOrDefault();

            if (banhTimThay != null)
            {
                return ExtractBanhInfo(banhTimThay, question);
            }

            return null;
        }

        // 🎯 Trích xuất thông tin phù hợp dựa trên câu hỏi
        private string ExtractBanhInfo(Banh banh, string question)
        {
            if (question.Contains("giá", System.StringComparison.OrdinalIgnoreCase))
                return $"💰 Giá của {banh.TenBanh} là {banh.Gia:N0} VND.";

            if (question.Contains("hạn sử dụng", System.StringComparison.OrdinalIgnoreCase) ||
                question.Contains("HSD", System.StringComparison.OrdinalIgnoreCase))
                return $"⏳ Hạn sử dụng của {banh.TenBanh} là {banh.Hsd:dd/MM/yyyy}.";

            if (question.Contains("ngày sản xuất", System.StringComparison.OrdinalIgnoreCase) ||
                question.Contains("NSX", System.StringComparison.OrdinalIgnoreCase))
                return $"📅 Ngày sản xuất của {banh.TenBanh} là {banh.Nsx:dd/MM/yyyy}.";

            if (question.Contains("mô tả", System.StringComparison.OrdinalIgnoreCase) ||
                question.Contains("giới thiệu", System.StringComparison.OrdinalIgnoreCase))
                return $"ℹ Mô tả về {banh.TenBanh}: {banh.Mota}.";

          

            return $"Bạn muốn biết thông tin gì về {banh.TenBanh}? (Giá, hạn sử dụng, ngày sản xuất, mô tả, hình ảnh,...)";
        }

        // 🎯 Thuật toán tìm bánh gần giống nhất
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

    public class ChatbotRequest
    {
        public string Question { get; set; }
    }
}
