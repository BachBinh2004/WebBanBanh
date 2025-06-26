using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanBanh.Models;

namespace WebBanBanh.Controllers
{
    public class CollectionsController : Controller
    {
        private readonly WebBanBanhContext _context;

        public CollectionsController(WebBanBanhContext context)
        {
            _context = context;
        }

        // GET: Collections
        public async Task<IActionResult> Index(string TenBanh, string LoaiBanh, string SapXep, int page = 1)
        {
            int pageSize = 6; // Số sản phẩm mỗi trang
            var webBanBanhContext = _context.Banhs.Include(b => b.MaLbNavigation).AsQueryable();

            int temp = 0; // Biến temp để xác định loại bánh

            // Lọc theo loại bánh (nếu có)
            if (!string.IsNullOrEmpty(LoaiBanh))
            {
                webBanBanhContext = webBanBanhContext.Where(b => b.MaLbNavigation.TenLb == LoaiBanh);

                // Gán temp theo loại bánh
                switch (LoaiBanh)
                {
                    case "Bánh Mì": temp = 1; break;
                    case "Bánh Ngọt": temp = 2; break;
                    case "Bánh Kem": temp = 3; break;
                    case "Bánh yêu cầu": temp = 4; break;
                    case "Bánh theo mùa": temp = 5; break;
                }
            }

            // Lọc theo tên bánh (nếu có)
            //if (!string.IsNullOrEmpty(TenBanh))
            //{
            //    webBanBanhContext = webBanBanhContext.Where(b => b.TenBanh.Contains(TenBanh));
            //}
            if (!string.IsNullOrEmpty(TenBanh))
            {
                string lowerSearch = TenBanh.ToLower();
                webBanBanhContext = webBanBanhContext.Where(b => b.TenBanh.ToLower().Contains(lowerSearch) || b.Mota.ToLower().Contains(lowerSearch));
            }
            // Xử lý sắp xếp theo giá hoặc tên
            switch (SapXep)
            {
                case "gia_asc":
                    webBanBanhContext = webBanBanhContext.OrderBy(b => b.Gia);
                    break;
                case "gia_desc":
                    webBanBanhContext = webBanBanhContext.OrderByDescending(b => b.Gia);
                    break;
                case "ten_asc":
                    webBanBanhContext = webBanBanhContext.OrderBy(b => b.TenBanh);
                    break;
                case "ten_desc":
                    webBanBanhContext = webBanBanhContext.OrderByDescending(b => b.TenBanh);
                    break;
            }

            // Phân trang
            var totalItems = await webBanBanhContext.CountAsync();
            var banhs = await webBanBanhContext.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            // Truyền dữ liệu vào ViewBag
            ViewBag.TenBanh = TenBanh;
            ViewBag.LoaiBanh = LoaiBanh;
            ViewBag.SapXep = SapXep;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            ViewBag.Temp = temp; // Truyền temp vào ViewBag

            return View(banhs);
        }



        // GET: Collections/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banh = await _context.Banhs
                .Include(b => b.MaLbNavigation)
                .FirstOrDefaultAsync(m => m.MaBanh == id);
            if (banh == null)
            {
                return NotFound();
            }

            return View(banh);
        }
        // GET: Collections/Create
        public IActionResult Create()
        {
            ViewData["MaLb"] = new SelectList(_context.LoaiBanhs, "MaLb", "MaLb");
            return View();
        }

        // POST: Collections/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MaBanh,TenBanh,Nsx,Hsd,Mota,Hinhanh,MaLb,Gia")] Banh banh)
        {
            if (ModelState.IsValid)
            {
                _context.Add(banh);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLb"] = new SelectList(_context.LoaiBanhs, "MaLb", "MaLb", banh.MaLb);
            return View(banh);
        }

        // GET: Collections/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banh = await _context.Banhs.FindAsync(id);
            if (banh == null)
            {
                return NotFound();
            }
            ViewData["MaLb"] = new SelectList(_context.LoaiBanhs, "MaLb", "MaLb", banh.MaLb);
            return View(banh);
        }

        // POST: Collections/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaBanh,TenBanh,Nsx,Hsd,Mota,Hinhanh,MaLb,Gia")] Banh banh)
        {
            if (id != banh.MaBanh)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(banh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BanhExists(banh.MaBanh))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaLb"] = new SelectList(_context.LoaiBanhs, "MaLb", "MaLb", banh.MaLb);
            return View(banh);
        }

        // GET: Collections/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banh = await _context.Banhs
                .Include(b => b.MaLbNavigation)
                .FirstOrDefaultAsync(m => m.MaBanh == id);
            if (banh == null)
            {
                return NotFound();
            }

            return View(banh);
        }

        // POST: Collections/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var banh = await _context.Banhs.FindAsync(id);
            if (banh != null)
            {
                _context.Banhs.Remove(banh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BanhExists(string id)
        {
            return _context.Banhs.Any(e => e.MaBanh == id);
        }
    }
}
