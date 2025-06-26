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
    public class BanhsController : Controller
    {
        private readonly WebBanBanhContext _context;

        public BanhsController(WebBanBanhContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> GioiThieu()
        {
            var webBanBanhContext = _context.Banhs.Include(b => b.MaLbNavigation);
            return View(await webBanBanhContext.ToListAsync());
        }
        // GET: Banhs
        public async Task<IActionResult> Index()
        {
            var webBanBanhContext = _context.Banhs.Include(b => b.MaLbNavigation);
            return View(await webBanBanhContext.ToListAsync());
        }
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
            ViewBag.banhLB2 = _context.Banhs.Where(b => b.MaLb == banh.MaLb).ToList();
            return View(banh);
        }
        public async Task<IActionResult> HoaDon(string id, int SoLuongBanh =1 )
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
            HttpContext.Session.SetInt32("SoLuongBanh", SoLuongBanh);
            int quantity = HttpContext.Session.GetInt32("SoLuongBanh") ?? 1;

            // Lấy mã hóa đơn mới nhất
            var allMaHd = _context.HoaDons.Select(hd => hd.MaHd).ToList();

            int newNumber = 1; // Mặc định nếu không có mã nào

            if (allMaHd.Count > 0)
            {
                // Lọc ra phần số từ mã hóa đơn và lấy số lớn nhất
                var maxNumber = allMaHd
                    .Where(mhd => mhd.StartsWith("HD")) // Chỉ lấy mã bắt đầu bằng "HD"
                    .Select(mhd =>
                    {
                        string numberPart = mhd.Substring(2); // Bỏ "HD"
                        return int.TryParse(numberPart, out int parsedNumber) ? parsedNumber : 0;
                    })
                    .Max(); // Lấy số lớn nhất

                newNumber = maxNumber + 1;
            }

            string newMaHd = "HD" + newNumber.ToString(); // Tạo mã mới

            // Tạo hóa đơn mới
            var hd = new HoaDon
            {
                MaHd = newMaHd,
                NgayLap = DateTime.Now,
                NgayGd = DateTime.Now,
                HinhThuc = "Lấy Tại Chỗ",
                MaKh = "KH1",
                MaNv = "NV1"
            };

            // Gán thông tin bánh vào ViewData để hiển thị
            ViewData["MaHd"] = newMaHd;
            ViewData["MaBanh"] = banh.MaBanh;
            ViewData["TenBanh"] = banh.TenBanh;
            ViewData["HinhAnh"] = banh.Hinhanh;
            ViewData["GiaBanh"] = banh.Gia;
            ViewData["Quantity"] = quantity;

            return View(hd);
        }
        [HttpPost]
        public async Task<IActionResult> QR(
                                             string MaHD,
                                             DateTime NgayLap,
                                             DateTime NgayGD,
                                             string HinhThuc,
                                             string MaKH,
                                             string MaNV,
                                             string MaBanh,
                                             int DGBan,
                                             int SL,
                                             List<string>? DSMaBanh , 
                                             List<int>? DSDGBan, 
                                             List<int>? DSSL)
        {
            // 1️⃣ Thêm vào bảng HoaDon
            int giatien = 0;
            var hoaDon = new HoaDon
            {
                MaHd = MaHD,
                NgayLap = NgayLap,
                NgayGd = NgayGD,
                HinhThuc = HinhThuc,
                MaKh = MaKH,
                MaNv = MaNV
            };
            _context.HoaDons.Add(hoaDon);
            await _context.SaveChangesAsync(); // Lưu vào database
            //if (DSDGBan == null || DSSL == null || DSMaBanh == null)
            if ((DSDGBan != null && DSDGBan.Count == 0) ||  (DSSL != null && DSSL.Count == 0) ||  (DSMaBanh != null && DSMaBanh.Count == 0))
            {
                var chiTietHoaDon = new Cthd
                {
                    MaHd = MaHD,  // Liên kết với bảng HoaDon
                    MaBanh = MaBanh,
                    Dgban = DGBan,
                    Sl = SL
                };
                _context.Cthds.Add(chiTietHoaDon);
                await _context.SaveChangesAsync(); // Lưu vào database
                giatien = DGBan * SL;
                return RedirectToAction("MaQR", "Banhs", new { giaTien = giatien });
            }
            else
            {
                foreach(var item in DSMaBanh)
                {
                    var chiTietHoaDon = new Cthd
                    {
                        MaHd = MaHD,  // Liên kết với bảng HoaDon
                        MaBanh = item,
                        Dgban = DSDGBan[DSMaBanh.IndexOf(item)],
                        Sl = DSSL[DSMaBanh.IndexOf(item)]
                    };
                    _context.Cthds.Add(chiTietHoaDon);
                    await _context.SaveChangesAsync(); // Lưu vào database
                    giatien += DSDGBan[DSMaBanh.IndexOf(item)] * DSSL[DSMaBanh.IndexOf(item)];
                }
                return RedirectToAction("MaQR", "Banhs", new { giaTien = giatien });
            }
            // 2️⃣ Thêm vào bảng CTHD (Chi Tiết Hóa Đơn)
            
            
            // 3️⃣ Chuyển hướng sau khi lưu thành công
            return RedirectToAction("MaQR", "Banhs", new { giaTien = giatien });

        }

        public ActionResult MaQR(int giatien)
        {
            ViewBag.giatien = giatien;
            return View();
        }
        // GET: Banhs/Create
        public IActionResult Create()
        {
            ViewData["MaLb"] = new SelectList(_context.LoaiBanhs, "MaLb", "MaLb");
            return View();
        }

        // POST: Banhs/Create
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

        // GET: Banhs/Edit/5
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

        // POST: Banhs/Edit/5
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

        // GET: Banhs/Delete/5
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

        // POST: Banhs/Delete/5
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
