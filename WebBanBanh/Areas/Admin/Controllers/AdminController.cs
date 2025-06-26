using System.Globalization;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebBanBanh.Models;

namespace Zota.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly WebBanBanhContext _context;
        public AdminController(WebBanBanhContext context)
        {
            _context = context;
        }
        // GET: Admin
        public async Task<IActionResult> Index(string selectedTable = "Cthd")
        {
            var tableNames = new Dictionary<string, string>
            {
                { "Banhs", "Bánh" },
                { "Nccs", "Nhà cung cấp" },
                { "HoaDons", "Hóa đơn" },
                { "DonDhs", "Đơn đặt hàng"   },
                { "KhachHangs", "Khách hàng" },
                { "LoaiBanhs", "Loại bánh" },
                { "LoaiKhs", "Loại khách hàng" },
                { "LoaiTks", "Loại tài khoản" },
                { "NguyenLieus", "Nguyên liệu" },
                { "NhanViens", "Nhân viên" },
                { "PhieuGhs", "Phiếu giao hàng" },
                { "PhieuNhs", "Phiếu nhập hàng" },
                { "TaiKhoans", "Tài khoản" },
                { "Ctddhs", "Chi tiết đơn đặt hàng" },
                { "Ctdnhs", "Chi tiết đơn nhập hàng" },
                { "Cthds", "Chi tiết hóa đơn" },
                { "Ctpghs", "Chi tiết phiếu giao hàng" }


            };

            IEnumerable<object> data = selectedTable switch
            {
                "Banhs" => await _context.Banhs
                   .Select(b => new { b.MaBanh, b.TenBanh, b.Nsx, b.Hsd, b.Mota, b.Hinhanh, b.MaLbNavigation.TenLb, b.Gia })
                   .ToListAsync(),
                "Ctddhs" => await _context.Ctddhs
                    .Select(b => new { b.MaNl, b.MaDdh, b.Dgmua, b.Sl })
                    .ToListAsync(),
                "Ctdnhs" => await _context.Ctdnhs
                    .Select(b => new { b.MaNl, b.MaDdh, b.MaPnh, b.Sl })
                    .ToListAsync(),
                "Cthds" => await _context.Cthds
                    .Select(b => new { b.MaBanh, b.MaHd, b.Dgban, b.Sl })
                    .ToListAsync(),
                "Ctpghs" => await _context.Ctpghs
                    .Select(b => new { b.MaBanh, b.MaPgh, b.Sl })
                    .ToListAsync(),
                "Nccs" => await _context.Nccs
                    .Select(b => new { b.MaNcc, b.TenNcc, b.Dcncc, b.Sdtncc })
                    .ToListAsync(),
                "HoaDons" => await _context.HoaDons
                    .Select(b => new { b.MaHd, b.NgayLap, b.NgayGd, b.HinhThuc, b.MaKh, b.MaNv })
                    .ToListAsync(),
                "KhachHangs" => await _context.KhachHangs
                    .Select(b => new { b.MaKh, b.TenKh, b.GioiTinh, b.Dckh, b.Sdtkh, b.MaLkh })
                    .ToListAsync(),
                "LoaiBanhs" => await _context.LoaiBanhs
                    .Select(b => new { b.MaLb, b.TenLb })
                    .ToListAsync(),
                "LoaiKhs" => await _context.LoaiKhs
                    .Select(b => new { b.MaLkh, b.TenLkh })
                    .ToListAsync(),
                "LoaiTks" => await _context.LoaiTks
                    .Select(b => new { b.MaLtk, b.TenLtk })
                    .ToListAsync(),
                "NguyenLieus" => await _context.NguyenLieus
                    .Select(b => new { b.MaNl, b.TenNl })
                    .ToListAsync(),
                "NhanViens" => await _context.NhanViens
                    .Select(b => new { b.MaNv, b.TenNv, b.GioiTinh, b.NgaySinh, b.ChucVu, b.Dcnv, b.Sdtnv })
                    .ToListAsync(),
                "PhieuGhs" => await _context.PhieuGhs
                    .Select(b => new { b.MaPgh, b.NgayGiao, b.MaHd, b.MaNv })
                    .ToListAsync(),
                "PhieuNhs" => await _context.PhieuNhs
                    .Select(b => new { b.MaPnh, b.NgayNh, b.MaDdh, b.MaNv })
                    .ToListAsync(),
                "TaiKhoans" => await _context.TaiKhoans
                    .Select(b => new { b.MaTk, b.MaLtk, b.Mk, b.TenDn })
                    .ToListAsync(),
                "DonDhs" => await _context.Ctddhs
                    .Select(b => new { b.MaNl, b.MaDdh, b.Dgmua, b.Sl })
                    .ToListAsync(),
                _ => new List<object>()
            };

            ViewData["TableNames"] = tableNames;
            ViewData["SelectedTable"] = selectedTable;
            return View(data);
        }



        // GET: Admin/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cthd = await _context.Cthds
                .Include(c => c.MaBanhNavigation)
                .Include(c => c.MaHdNavigation)
                .FirstOrDefaultAsync(m => m.MaBanh == id);
            if (cthd == null)
            {
                return NotFound();
            }

            return View(cthd);
        }
        // GET: Admin/Create
        [HttpGet("Admin/Admin/Create/{table}")]
        public IActionResult Create(string table)
        {
            if (string.IsNullOrEmpty(table)) return NotFound("Bảng không hợp lệ.");

            ViewData["Table"] = table;

            // Sinh mã ID tự động (nếu cần)
            string? generatedId = table switch
            {
                "Banhs" => GenerateNextId<Banh>("B", "MaBanh"),
                "KhachHangs" => GenerateNextId<KhachHang>("KH", "MaKh"),
                "NhanViens" => GenerateNextId<NhanVien>("NV", "MaNv"),
                "HoaDons" => GenerateNextId<HoaDon>("HD", "MaHd"),
                "LoaiBanhs" => GenerateNextId<LoaiBanh>("LB", "MaLb"),
                "LoaiKhs" => GenerateNextId<LoaiKh>("LKH", "MaLkh"),
                "LoaiTks" => GenerateNextId<LoaiTk>("LTK", "MaLtk"),
                "NguyenLieus" => GenerateNextId<NguyenLieu>("NL", "MaNl"),
                "PhieuGhs" => GenerateNextId<PhieuGh>("PGH", "MaPgh"),
                "DonDhs" => GenerateNextId<DonDh>("DDH","MaDdh"),
                "PhieuNhs" => GenerateNextId<PhieuNh>("PNH", "MaPnh"),
                "TaiKhoans" => GenerateNextId<TaiKhoan>("TK", "MaTk"),
                "Nccs" => GenerateNextId<Ncc>("NCC", "MaNcc"),
                // các bảng không cần ID tự động
                "Ctddhs" => null,
                "Ctdnhs" => null,
                "Cthds" => null,
                "Ctpghs" => null,
                _ => null
            };

            if (generatedId != null)
            {
                ViewData["GeneratedId"] = generatedId;
            }

            // Load ViewBag tùy bảng (nếu có dropdown)
            ViewBag.LoaiBanhs = _context.LoaiBanhs.ToList();
            ViewBag.LoaiKhs = _context.LoaiKhs.ToList();
            ViewBag.LoaiTks = _context.LoaiTks.ToList();
            ViewBag.KhachHangs = _context.KhachHangs.ToList();
            ViewBag.NhanViens = _context.NhanViens.ToList();
            ViewBag.Banhs = _context.Banhs.ToList();
            ViewBag.HoaDons = _context.HoaDons.ToList();
            ViewBag.NguyenLieus = _context.NguyenLieus.ToList();
            ViewBag.Nccs = _context.Nccs.ToList();
            ViewBag.PhieuNhs = _context.PhieuNhs.ToList();
            ViewBag.DonDhs = _context.DonDhs.ToList();
            ViewBag.PhieuGhs = _context.PhieuGhs.ToList();
            ViewBag.TaiKhoans = _context.TaiKhoans.ToList();

            return View();
        }

        [HttpPost("Admin/Admin/Create/{table}")]
        public IActionResult Create(string table, Dictionary<string, string> formData)
        {
            if (string.IsNullOrEmpty(table)) return NotFound("Bảng không hợp lệ.");

            try
            {
                object? newRecord = table switch
                {
                    "Banhs" => new Banh
                    {
                        MaBanh = formData["MaBanh"],
                        TenBanh = formData["TenBanh"],
                        Nsx = DateTime.Parse(formData["Nsx"]),
                        Hsd = DateTime.Parse(formData["Hsd"]),
                        Mota = formData["Mota"],
                        Hinhanh = formData["Hinhanh"],
                        Gia = int.Parse(formData["Gia"]),
                        MaLb = formData["TenLb"]
                    },
                    "KhachHangs" => new KhachHang
                    {
                        MaKh = formData["MaKh"],
                        TenKh = formData["TenKh"],
                        GioiTinh = formData["GioiTinh"],
                        Dckh = formData["Dckh"],
                        Sdtkh = formData["Sdtkh"],
                        MaLkh = formData["MaLkh"]
                    },
                    "NhanViens" => new NhanVien
                    {
                        MaNv = formData["MaNv"],
                        TenNv = formData["TenNv"],
                        GioiTinh = formData["GioiTinh"],
                        NgaySinh = DateTime.Parse(formData["NgaySinh"]),
                        ChucVu = formData["ChucVu"],
                        Dcnv = formData["Dcnv"],
                        Sdtnv = formData["Sdtnv"]
                    },
                    "HoaDons" => new HoaDon
                    {
                        MaHd = formData["MaHd"],
                        NgayLap = DateTime.Parse(formData["NgayLap"]),
                        NgayGd = DateTime.Parse(formData["NgayGd"]),
                        HinhThuc = formData["HinhThuc"],
                        MaKh = formData["MaKh"],
                        MaNv = formData["MaNv"]
                    },
                    "LoaiBanhs" => new LoaiBanh
                    {
                        MaLb = formData["MaLb"],
                        TenLb = formData["TenLb"]
                    },
                    "LoaiKhs" => new LoaiKh
                    {
                        MaLkh = formData["MaLkh"],
                        TenLkh = formData["TenLkh"]
                    },
                    "LoaiTks" => new LoaiTk
                    {
                        MaLtk = formData["MaLtk"],
                        TenLtk = formData["TenLtk"]
                    },
                    "NguyenLieus" => new NguyenLieu
                    {
                        MaNl = formData["MaNl"],
                        TenNl = formData["TenNl"]
                    },
                    "PhieuGhs" => new PhieuGh
                    {
                        MaPgh = formData["MaPgh"],
                        NgayGiao = DateTime.Parse(formData["NgayGiao"]),
                        MaHd = formData["MaHd"],
                        MaNv = formData["MaNv"]
                    },
                    "PhieuNhs" => new PhieuNh
                    {
                        MaPnh = formData["MaPnh"],
                        NgayNh = DateTime.Parse(formData["NgayNh"]),
                        MaDdh = formData["MaDdh"],
                        MaNv = formData["MaNv"]
                    },
                    "TaiKhoans" => new TaiKhoan
                    {
                        MaTk = formData["MaTk"],
                        MaLtk = formData["MaLtk"],
                        Mk = formData["Mk"],
                        TenDn = formData["TenDn"]
                    },
                    "Ctddhs" => new Ctddh
                    {
                        MaNl = formData["MaNl"],
                        MaDdh = formData["MaDdh"],
                        Dgmua = int.Parse(formData["Dgmua"]),
                        Sl = int.Parse(formData["Sl"])
                    },
                    "Ctdnhs" => new Ctdnh
                    {
                        MaNl = formData["MaNl"],
                        MaDdh = formData["MaDdh"],
                        MaPnh = formData["MaPnh"],
                        Sl = int.Parse(formData["Sl"])
                    },
                    "Cthds" => new Cthd
                    {
                        MaBanh = formData["MaBanh"],
                        MaHd = formData["MaHd"],
                        Dgban = int.Parse(formData["Dgban"]),
                        Sl = int.Parse(formData["Sl"])
                    },
                    "Ctpghs" => new Ctpgh
                    {
                        MaBanh = formData["MaBanh"],
                        MaPgh = formData["MaPgh"],
                        Sl = int.Parse(formData["Sl"])
                    },
                    "Nccs" => new Ncc
                    {
                        MaNcc = formData["MaNcc"],
                        TenNcc = formData["TenNcc"],
                        Dcncc = formData["Dcncc"],
                        Sdtncc = formData["Sdtncc"]
                    },
                    "DonDhs" => new DonDh
                    {
                        MaDdh = formData["MaDdh"],
                        NgayDh = DateTime.Parse(formData["NgayDh"]),
                        NgayGiao = DateTime.Parse(formData["NgayGiao"]),
                        MaNcc = formData["MaNcc"],
                        MaNv = formData["MaNv"]
                    },
                    _ => null
                };

                if (newRecord == null) return NotFound("Không hỗ trợ bảng này.");

                _context.Add(newRecord);
                _context.SaveChanges();

                return RedirectToAction("Index", new { selectedTable = table });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi thêm dữ liệu: {ex.Message}");
            }
        }

        [HttpGet("Admin/Admin/Edit/{table}/{id}")]
        public IActionResult Edit(string table, string id)
        {
            if (string.IsNullOrEmpty(table) || id == "")
            {
                return NotFound();
            }

            object record = null;

            switch (table)
            {

                case "Banhs":
                    record = _context.Banhs.FirstOrDefault(b => b.MaBanh == id);
                    break;
                case "Ctddhs":
                    record = _context.Ctddhs.FirstOrDefault(b => b.MaNl == id);
                    break;
                case "NhanViens":
                    record = _context.NhanViens.FirstOrDefault(b => b.MaNv == id);
                    break;
                case "Ddhs":
                    record = _context.DonDhs.FirstOrDefault(b => b.MaDdh == id);
                    break;
                case "Ctdnhs":
                    record = _context.Ctdnhs.FirstOrDefault(b => b.MaNl == id);
                    break;
                case "Cthds":
                    record = _context.Cthds.FirstOrDefault(b => b.MaBanh == id);
                    break;
                case "Ctpghs":
                    record = _context.Ctpghs.FirstOrDefault(b => b.MaBanh == id);
                    break;
                case "Nccs":
                    record = _context.Nccs.FirstOrDefault(b => b.MaNcc == id);
                    break;
                case "HoaDons":
                    record = _context.HoaDons.FirstOrDefault(b => b.MaHd == id);
                    break;
                case "KhachHangs":
                    record = _context.KhachHangs.FirstOrDefault(b => b.MaKh == id);
                    break;
                case "LoaiBanhs":
                    record = _context.LoaiBanhs.FirstOrDefault(b => b.MaLb == id);
                    break;
                case "LoaiKhs":
                    record = _context.LoaiKhs.FirstOrDefault(b => b.MaLkh == id);
                    break;
                case "LoaiTks":
                    record = _context.LoaiTks.FirstOrDefault(b => b.MaLtk == id);
                    break;
                case "NguyenLieus":
                    record = _context.NguyenLieus.FirstOrDefault(b => b.MaNl == id);
                    break;
                case "PhieuGhs":
                    record = _context.PhieuGhs.FirstOrDefault(b => b.MaPgh == id);
                    break;
                case "PhieuNhs":
                    record = _context.PhieuNhs.FirstOrDefault(b => b.MaPnh == id);
                    break;
                case "TaiKhoans":
                    record = _context.TaiKhoans.FirstOrDefault(b => b.MaTk == id);
                    break;


                // Thêm các bảng khác tương tự
                default:
                    return NotFound();
            }

            if (record == null)
            {
                return NotFound();
            }
            ViewData["Table"] = table;
            return View(record);
        }

        [HttpPost("Admin/Admin/Edit/{table}/{id}")]
        public IActionResult Edit(string table, string id, IFormCollection form)
        {
            if (string.IsNullOrEmpty(table) || id == null)
            {
                return BadRequest();
            }

            object? record = null;

            switch (table)
            {
                case "Banhs":
                    record = _context.Banhs.FirstOrDefault(b => b.MaBanh == id);
                    break;
                case "Ctddhs":
                    record = _context.Ctddhs.FirstOrDefault(b => b.MaNl == id);
                    break;
                case "NhanViens":
                    record = _context.NhanViens.FirstOrDefault(b => b.MaNv == id);
                    break;
                case "Ddhs":
                    record = _context.DonDhs.FirstOrDefault(b => b.MaDdh == id);
                    break;
                case "Ctdnhs":
                    record = _context.Ctdnhs.FirstOrDefault(b => b.MaNl == id);
                    break;
                case "Cthds":
                    record = _context.Cthds.FirstOrDefault(b => b.MaBanh == id);
                    break;
                case "Ctpghs":
                    record = _context.Ctpghs.FirstOrDefault(b => b.MaBanh == id);
                    break;
                case "Nccs":
                    record = _context.Nccs.FirstOrDefault(b => b.MaNcc == id);
                    break;
                case "HoaDons":
                    record = _context.HoaDons.FirstOrDefault(b => b.MaHd == id);
                    break;
                case "KhachHangs":
                    record = _context.KhachHangs.FirstOrDefault(b => b.MaKh == id);
                    break;
                case "LoaiBanhs":
                    record = _context.LoaiBanhs.FirstOrDefault(b => b.MaLb == id);
                    break;
                case "LoaiKhs":
                    record = _context.LoaiKhs.FirstOrDefault(b => b.MaLkh == id);
                    break;
                case "LoaiTks":
                    record = _context.LoaiTks.FirstOrDefault(b => b.MaLtk == id);
                    break;
                case "NguyenLieus":
                    record = _context.NguyenLieus.FirstOrDefault(b => b.MaNl == id);
                    break;
                case "PhieuGhs":
                    record = _context.PhieuGhs.FirstOrDefault(b => b.MaPgh == id);
                    break;
                case "PhieuNhs":
                    record = _context.PhieuNhs.FirstOrDefault(b => b.MaPnh == id);
                    break;
                case "TaiKhoans":
                    record = _context.TaiKhoans.FirstOrDefault(b => b.MaTk == id);
                    break;
                default:
                    return NotFound();
            }

            if (record == null)
            {
                return NotFound();
            }

            // phần cập nhật code ở đây, cập nhật record theo form
            var properties = record.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (form.ContainsKey(property.Name))
                {
                    var value = form[property.Name].ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        try
                        {
                            var targetType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                            object convertedValue;

                            // Xử lý Enum
                            if (targetType.IsEnum)
                            {
                                convertedValue = Enum.Parse(targetType, value, true);
                            }
                            // Xử lý DateTime
                            else if (targetType == typeof(DateTime))
                            {
                                if (!DateTime.TryParse(value, out DateTime parsedDate))
                                {
                                    return BadRequest($"Lỗi khi cập nhật {property.Name}, ngày tháng không hợp lệ.");
                                }
                                convertedValue = parsedDate;
                            }
                            // Xử lý Guid
                            else if (targetType == typeof(Guid))
                            {
                                if (!Guid.TryParse(value, out Guid parsedGuid))
                                {
                                    return BadRequest($"Lỗi khi cập nhật {property.Name}, GUID không hợp lệ.");
                                }
                                convertedValue = parsedGuid;
                            }
                            // Xử lý Boolean
                            else if (targetType == typeof(bool))
                            {
                                if (!bool.TryParse(value, out bool parsedBool))
                                {
                                    return BadRequest($"Lỗi khi cập nhật {property.Name}, giá trị boolean không hợp lệ.");
                                }
                                convertedValue = parsedBool;
                            }
                            // Xử lý số nguyên (int, long, short, byte)
                            else if (targetType == typeof(int) || targetType == typeof(long) ||
                                     targetType == typeof(short) || targetType == typeof(byte))
                            {
                                if (!long.TryParse(value, out long parsedInt))
                                {
                                    return BadRequest($"Lỗi khi cập nhật {property.Name}, giá trị số nguyên không hợp lệ.");
                                }
                                convertedValue = Convert.ChangeType(parsedInt, targetType);
                            }
                            // Xử lý số thực (float, double, decimal)
                            else if (targetType == typeof(float) || targetType == typeof(double) || targetType == typeof(decimal))
                            {
                                if (!decimal.TryParse(value, out decimal parsedDecimal))
                                {
                                    return BadRequest($"Lỗi khi cập nhật {property.Name}, giá trị số thực không hợp lệ.");
                                }
                                convertedValue = Convert.ChangeType(parsedDecimal, targetType);
                            }
                            // Xử lý chuỗi
                            else if (targetType == typeof(string))
                            {
                                convertedValue = value;
                            }
                            // Xử lý các kiểu còn lại
                            else
                            {
                                convertedValue = Convert.ChangeType(value, targetType);
                            }

                            property.SetValue(record, convertedValue);
                        }
                        catch (Exception ex)
                        {
                            return BadRequest($"Lỗi khi cập nhật trường {property.Name}: {ex.Message}");
                        }
                    }

                }
            }
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: Admin/Admin/Delete/{table}/{id}
        [HttpGet("Admin/Admin/Delete/{table}/{id}")]
        public IActionResult Delete(string table, string id)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(id)) return NotFound();

            object? record = table switch
            {
                "Banhs" => _context.Banhs.Find(id),
                "Ctddhs" => _context.Ctddhs.Find(id),
                "NhanViens" => _context.NhanViens.Find(id),
                "Ctdnhs" => _context.Ctdnhs.Find(id),
                "Cthds" => _context.Cthds
                                .Include(c => c.MaBanhNavigation)
                                .Include(c => c.MaHdNavigation)
                                .FirstOrDefault(c => c.MaBanh == id),
                "Ctpghs" => _context.Ctpghs.Find(id),
                "Nccs" => _context.Nccs.Find(id),
                "HoaDons" => _context.HoaDons.Find(id),
                "KhachHangs" => _context.KhachHangs.Find(id),
                "LoaiBanhs" => _context.LoaiBanhs.Find(id),
                "LoaiKhs" => _context.LoaiKhs.Find(id),
                "LoaiTks" => _context.LoaiTks.Find(id),
                "NguyenLieus" => _context.NguyenLieus.Find(id),
                "PhieuGhs" => _context.PhieuGhs.Find(id),
                "PhieuNhs" => _context.PhieuNhs.Find(id),
                "TaiKhoans" => _context.TaiKhoans.Find(id),
                _ => null
            };

            if (record == null) return NotFound();

            ViewData["Table"] = table;
            return View(record);
        }


        // POST: Admin/Admin/Delete/{table}/{id}
        [HttpPost("Admin/Admin/Delete/{table}/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDelete(string table, string id)
        {
            if (string.IsNullOrEmpty(table) || string.IsNullOrEmpty(id)) return BadRequest();

            object? record = table switch
            {
                "Banhs" => _context.Banhs.Find(id),
                "Ctddhs" => _context.Ctddhs.Find(id),
                "NhanViens" => _context.NhanViens.Find(id),
                "Ctdnhs" => _context.Ctdnhs.Find(id),
                "Cthds" => _context.Cthds.Find(id),
                "Ctpghs" => _context.Ctpghs.Find(id),
                "Nccs" => _context.Nccs.Find(id),
                "HoaDons" => _context.HoaDons.Find(id),
                "KhachHangs" => _context.KhachHangs.Find(id),
                "LoaiBanhs" => _context.LoaiBanhs.Find(id),
                "LoaiKhs" => _context.LoaiKhs.Find(id),
                "LoaiTks" => _context.LoaiTks.Find(id),
                "NguyenLieus" => _context.NguyenLieus.Find(id),
                "PhieuGhs" => _context.PhieuGhs.Find(id),
                "PhieuNhs" => _context.PhieuNhs.Find(id),
                "TaiKhoans" => _context.TaiKhoans.Find(id),
                _ => null
            };

            if (record == null) return NotFound();

            _context.Remove(record);
            _context.SaveChanges();

            return RedirectToAction("Index", new { selectedTable = table });
        }


        private string GenerateNextId<TEntity>(string prefix, string keyPropertyName) where TEntity : class
        {
            var set = _context.Set<TEntity>();
            var idList = set
                .AsQueryable()
                .Select(e => EF.Property<string>(e, keyPropertyName))
                .Where(id => id.StartsWith(prefix))
                .ToList();

            int max = 0;
            foreach (var id in idList)
            {
                if (int.TryParse(id.Substring(prefix.Length), out int num))
                {
                    if (num > max) max = num;
                }
            }

            return prefix + (max + 1).ToString("D3"); // b001, nv005,...
        }



        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromAddress = new MailAddress("shinigami3003@gmail.com", "Zota Hotel");
            var toAddress = new MailAddress(toEmail);
            const string fromPassword = "rhop ystw cyrl nagp"; // <- thay bằng App Password

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            };
            smtp.Send(message);
        }


        [HttpGet("Admin/Admin/Dashboard")]
        public IActionResult Dashboard()
        {
            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            // THỐNG KÊ TỔNG QUÁT
            var banhs = _context.Banhs.ToList();
            var types = _context.LoaiBanhs.ToList();
            var customers = _context.KhachHangs.ToList();
            var employees = _context.NhanViens.ToList();
            var suppliers = _context.Nccs.ToList();

            ViewBag.TotalBanhs = banhs.Count;
            ViewBag.TotalTypes = types.Count;
            ViewBag.TotalCustomers = customers.Count;
            ViewBag.TotalEmployees = employees.Count;
            ViewBag.TotalSuppliers = suppliers.Count;

            ViewBag.BanhsList = banhs.Select(b => new { b.MaBanh, b.TenBanh, b.Gia }).ToList();
            ViewBag.TypesList = types.Select(t => new { t.MaLb, t.TenLb }).ToList();
            ViewBag.CustomersList = customers.Select(k => new { k.MaKh, k.TenKh, k.Sdtkh }).ToList();
            ViewBag.EmployeesList = employees.Select(n => new { n.MaNv, n.TenNv, n.ChucVu }).ToList();
            ViewBag.SuppliersList = suppliers.Select(ncc => new { ncc.MaNcc, ncc.TenNcc, ncc.Sdtncc }).ToList();



            // HÀNG TỒN KHO

            ViewBag.LowStockNguyenLieu = _context.Ctdnhs
                .Where(n => n.Sl < 10)
                .Select(n => new { n.MaNl, n.Sl })
                .ToList();

            // DOANH THU
            var cthds = _context.Cthds.Include(c => c.MaHdNavigation).ToList();

            ViewBag.DailyRevenue = cthds
                .Where(c => c.MaHdNavigation.NgayLap.Date == today)
                .Sum(c => (decimal?)c.Dgban * c.Sl) ?? 0;

            ViewBag.MonthlyRevenue = cthds
                .Where(c => c.MaHdNavigation.NgayLap.Month == currentMonth && c.MaHdNavigation.NgayLap.Year == currentYear)
                .Sum(c => (decimal?)c.Dgban * c.Sl) ?? 0;

            ViewBag.YearlyRevenue = cthds
                .Where(c => c.MaHdNavigation.NgayLap.Year == currentYear)
                .Sum(c => (decimal?)c.Dgban * c.Sl) ?? 0;

            ViewBag.MonthlyRevenueChart = cthds
                .GroupBy(c => new { c.MaHdNavigation.NgayLap.Year, c.MaHdNavigation.NgayLap.Month })
                .Select(g => new {
                    Thang = $"{g.Key.Month:D2}/{g.Key.Year}",
                    DoanhThu = g.Sum(x => x.Dgban * x.Sl)
                })
                .OrderBy(x => x.Thang)
                .ToDictionary(x => x.Thang, x => x.DoanhThu);

            // BÁNH BÁN CHẠY
            ViewBag.TopProducts = cthds
                .GroupBy(c => c.MaBanh)
                .Select(g => new {
                    MaBanh = g.Key,
                    TenBanh = _context.Banhs.FirstOrDefault(b => b.MaBanh == g.Key).TenBanh,
                    Sold = g.Sum(x => x.Sl)
                })
                .OrderByDescending(x => x.Sold)
                .Take(5)
                .ToList();

            // ĐƠN HÀNG
            var hoaDons = _context.HoaDons.ToList();
            ViewBag.TotalOrders = hoaDons.Count;
            ViewBag.PendingOrders = hoaDons.Count(h => h.HinhThuc == "Đặt bánh");
            ViewBag.DeliveredOrders = hoaDons.Count(h => h.HinhThuc == "Đã giao");
            ViewBag.CancelledOrders = hoaDons.Count(h => h.HinhThuc == "Hủy");
            ViewBag.TakedRate = hoaDons.Count == 0 ? 0 : Math.Round(ViewBag.PendingOrders * 100.0 / hoaDons.Count, 2);

            // KHÁCH HÀNG


            ViewBag.LoyalCustomers = _context.HoaDons
                .GroupBy(h => h.MaKh)
                .Where(g => g.Count() >= 3)
                .Select(g => new {
                    MaKh = g.Key,
                    TenKh = _context.KhachHangs.FirstOrDefault(k => k.MaKh == g.Key).TenKh,
                    SoLanMua = g.Count()
                })
                .ToList();


            ViewBag.TopCustomers = _context.HoaDons
                .GroupBy(h => h.MaKh)
                .Select(g => new {
                    MaKh = g.Key,
                    TenKh = _context.KhachHangs.FirstOrDefault(k => k.MaKh == g.Key).TenKh,
                    TotalOrders = g.Count()
                })
                .OrderByDescending(x => x.TotalOrders)
                .Take(5)
                .ToList();

            // NHÂN VIÊN
            ViewBag.EmployeePerformance = _context.HoaDons
                .GroupBy(h => h.MaNv)
                .Select(g => new {
                    MaNv = g.Key,
                    TenNv = _context.NhanViens.FirstOrDefault(n => n.MaNv == g.Key).TenNv,
                    OrdersHandled = g.Count()
                })
                .OrderByDescending(x => x.OrdersHandled)
                .ToList();

            return View();
        }

        //FINAL UPDATE

    }
}
