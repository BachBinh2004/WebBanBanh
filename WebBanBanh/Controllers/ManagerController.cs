using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanBanh.Models;

namespace WebBanBanh.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        private readonly WebBanBanhContext _context;

        public ManagerController(WebBanBanhContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "Admin")]
        // GET: Manager
        public async Task<IActionResult> Index(string selectedTable = "Cthd")
        {
            var tableNames = new Dictionary<string, string>
            {
                { "Banhs", "Bánh" },
                { "Ctddhs", "Chi tiết đơn đặt hàng" },
                { "Ctdnhs", "Chi tiết đơn nhập hàng" },
                { "Cthds", "Chi tiết hóa đơn" },
                { "Ctpghs", "Chi tiết phiếu giao hàng" },
                { "Nccs", "Nhà cung cấp" },
                { "HoaDons", "Hóa đơn" },
                { "KhachHangs", "Khách hàng" },
                { "LoaiBanhs", "Loại bánh" },
                { "LoaiKhs", "Loại khách hàng" },
                { "LoaiTks", "Loại tài khoản" },
                { "NguyenLieus", "Nguyên liệu" },
                { "NhanViens", "Nhân viên" },
                { "PhieuGhs", "Phiếu giao hàng" },
                { "PhieuNhs", "Phiếu nhập hàng" },
                { "TaiKhoans", "Tài khoản" }
            };

            IEnumerable<object> data = selectedTable switch
            {
                "Banhs" => await _context.Banhs
                   .Select(b => new { b.MaBanh, b.TenBanh, b.Nsx, b.Hsd, b.Mota, b.Hinhanh, b.MaLbNavigation.TenLb, b.Gia  }) 
                   .ToListAsync(),
                "Ctddhs" => await _context.Ctddhs
                    .Select(b => new { b.MaNl , b.MaDdh, b.Dgmua, b.Sl })
                    .ToListAsync(),
                "Ctdnhs" => await _context.Ctdnhs
                    .Select(b => new { b.MaNl , b.MaDdh, b.MaPnh, b.Sl })
                    .ToListAsync(),
                "Cthds" => await _context.Cthds
                    .Select(b => new { b.MaBanh , b.MaHd, b.Dgban, b.Sl })
                    .ToListAsync(),
                "Ctpghs" => await _context.Ctpghs
                    .Select(b => new { b.MaBanh , b.MaPgh, b.Sl })
                    .ToListAsync(),
                "Nccs" => await _context.Nccs
                    .Select(b => new { b.MaNcc , b.TenNcc, b.Dcncc, b.Sdtncc })
                    .ToListAsync(),
                "HoaDons" => await _context.HoaDons
                    .Select(b => new { b.MaHd , b.NgayLap, b.NgayGd, b.HinhThuc, b.MaKh, b.MaNv })
                    .ToListAsync(),
                "KhachHangs" => await _context.KhachHangs
                    .Select(b => new { b.MaKh , b.TenKh, b.GioiTinh, b.Dckh, b.Sdtkh, b.MaLkh })
                    .ToListAsync(),
                "LoaiBanhs" => await _context.LoaiBanhs
                    .Select(b => new { b.MaLb , b.TenLb })
                    .ToListAsync(),
                "LoaiKhs" => await _context.LoaiKhs
                    .Select(b => new { b.MaLkh , b.TenLkh })
                    .ToListAsync(),
                "LoaiTks" => await _context.LoaiTks
                    .Select(b => new { b.MaLtk , b.TenLtk })
                    .ToListAsync(),
                "NguyenLieus" => await _context.NguyenLieus
                    .Select(b => new { b.MaNl , b.TenNl })
                    .ToListAsync(),
                "NhanViens" => await _context.NhanViens
                    .Select(b => new { b.MaNv , b.TenNv, b.GioiTinh, b.NgaySinh, b.ChucVu, b.Dcnv, b.Sdtnv })
                    .ToListAsync(),
                "PhieuGhs" => await _context.PhieuGhs
                    .Select(b => new { b.MaPgh , b.NgayGiao, b.MaHd, b.MaNv })
                    .ToListAsync(),
                "PhieuNhs" => await _context.PhieuNhs
                    .Select(b => new { b.MaPnh , b.NgayNh, b.MaDdh, b.MaNv })
                    .ToListAsync(),
                "TaiKhoans" => await _context.TaiKhoans
                    .Select(b => new { b.MaTk , b.MaLtk, b.Mk, b.TenDn })
                    .ToListAsync(),
                _ => new List<object>()
            };

            ViewData["TableNames"] = tableNames;
            ViewData["SelectedTable"] = selectedTable;
            return View(data);
        }


        // GET: Manager/Details/5
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

        // GET: Manager/Create
        [Route("Manager/Create/{table}")] // định tuyến để nhận table từ url
        public IActionResult Create(string table)
        {
            if (string.IsNullOrEmpty(table))
            {
                return NotFound("Bảng không hợp lệ.");
            }

            ViewData["Table"] = table;
            return View();
        }
        [HttpPost]
        public IActionResult Create(string table, Dictionary<string, string> formData)
        {
            if (string.IsNullOrEmpty(table))
            {
                return NotFound("Bảng không hợp lệ.");
            }

            try
            {
                // Chuyển dữ liệu thành object theo bảng tương ứng
                object newRecord = null;
                switch (table)
                {
                    case "Banhs":
                        newRecord = new Banh
                        {
                            MaBanh = formData["MaBanh"],
                            TenBanh = formData["TenBanh"],
                            Nsx = DateTime.Parse(formData["Nsx"]),
                            Hsd = DateTime.Parse(formData["Hsd"]),
                            Mota = formData["Mota"],
                            Hinhanh = formData["Hinhanh"],
                            Gia = int.Parse(formData["Gia"]),
                            MaLb = formData["TenLb"]
                        };
                        break;

                    case "KhachHangs":
                        newRecord = new KhachHang
                        {
                            MaKh = formData["MaKh"],
                            TenKh = formData["TenKh"],
                            GioiTinh = formData["GioiTinh"],
                            Dckh = formData["Dckh"],
                            Sdtkh = formData["Sdtkh"],
                            MaLkh = formData["MaLkh"]
                        };
                        break;

                    case "NhanViens":
                        newRecord = new NhanVien
                        {
                            MaNv = formData["MaNv"],
                            TenNv = formData["TenNv"],
                            GioiTinh = formData["GioiTinh"],
                            NgaySinh = DateTime.Parse(formData["NgaySinh"]),
                            ChucVu = formData["ChucVu"],
                            Dcnv = formData["Dcnv"],
                            Sdtnv = formData["Sdtnv"]
                        };
                        break;
                    //Tạo thêm cho tôi tất cả các case khác nữa
                    case "Ctddhs":
                        newRecord = new Ctddh
                        {
                            MaNl = formData["MaNl"],
                            MaDdh = formData["MaDdh"],
                            Dgmua = int.Parse(formData["Dgmua"]),
                            Sl = int.Parse(formData["Sl"])
                        };
                        break;
                    case "Ctdnhs":
                        newRecord = new Ctdnh
                        {
                            MaNl = formData["MaNl"],
                            MaDdh = formData["MaDdh"],
                            MaPnh = formData["MaPnh"],
                            Sl = int.Parse(formData["Sl"])
                        };
                        break;
                    case "Cthds":
                        newRecord = new Cthd
                        {
                            MaBanh = formData["MaBanh"],
                            MaHd = formData["MaHd"],
                            Dgban = int.Parse(formData["Dgban"]),
                            Sl = int.Parse(formData["Sl"])
                        };
                        break;
                    case "Ctpghs":
                        newRecord = new Ctpgh
                        {
                            MaBanh = formData["MaBanh"],
                            MaPgh = formData["MaPgh"],
                            Sl = int.Parse(formData["Sl"])
                        };
                        break;
                    case "Nccs":
                        newRecord = new Ncc
                        {
                            MaNcc = formData["MaNcc"],
                            TenNcc = formData["TenNcc"],
                            Dcncc = formData["Dcncc"],
                            Sdtncc = formData["Sdtncc"]
                        };
                        break;
                    case "HoaDons":
                        newRecord = new HoaDon
                        {
                            MaHd = formData["MaHd"],
                            NgayLap = DateTime.Parse(formData["NgayLap"]),
                            NgayGd = DateTime.Parse(formData["NgayGd"]),
                            HinhThuc = formData["HinhThuc"],
                            MaKh = formData["MaKh"],
                            MaNv = formData["MaNv"]
                        };
                        break;
                    case "LoaiBanhs":
                        newRecord = new LoaiBanh
                        {
                            MaLb = formData["MaLb"],
                            TenLb = formData["TenLb"]
                        };
                        break;
                    case "LoaiKhs":
                        newRecord = new LoaiKh
                        {
                            MaLkh = formData["MaLkh"],
                            TenLkh = formData["TenLkh"]
                        };
                        break;
                    case "LoaiTks":
                        newRecord = new LoaiTk
                        {
                            MaLtk = formData["MaLtk"],
                            TenLtk = formData["TenLtk"]
                        };
                        break;
                    case "NguyenLieus":
                        newRecord = new NguyenLieu
                        {
                            MaNl = formData["MaNl"],
                            TenNl = formData["TenNl"]
                        };
                        break;
                    case "PhieuGhs":
                        newRecord = new PhieuGh
                        {
                            MaPgh = formData["MaPgh"],
                            NgayGiao = DateTime.Parse(formData["NgayGiao"]),
                            MaHd = formData["MaHd"],
                            MaNv = formData["MaNv"]
                        };
                        break;
                    case "PhieuNhs":
                        newRecord = new PhieuNh
                        {
                            MaPnh = formData["MaPnh"],
                            NgayNh = DateTime.Parse(formData["NgayNh"]),
                            MaDdh = formData["MaDdh"],
                            MaNv = formData["MaNv"]
                        };
                        break;
                    case "TaiKhoans":
                        newRecord = new TaiKhoan
                        {
                            MaTk = formData["MaTk"],
                            MaLtk = formData["MaLtk"],
                            Mk = formData["Mk"],
                            TenDn = formData["TenDn"]
                        };
                        break;
                    default:
                        return NotFound("Không hỗ trợ bảng này.");
                }

                if (newRecord != null)
                {
                    _context.Add(newRecord);
                    _context.SaveChanges();
                }

                return RedirectToAction("Index", new { selectedTable = table });
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi thêm dữ liệu: {ex.Message}");
            }
        }

        // GET: Manager/Edit/5
        [Route("Manager/Edit/{table}/{id}")]
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

            [HttpPost]
            [Route("Manager/Edit/{table}/{id}")]
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
        // GET: Manager/Delete/5
        public async Task<IActionResult> Delete(string id)
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

        // POST: Manager/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var cthd = await _context.Cthds.FindAsync(id);
            if (cthd != null)
            {
                _context.Cthds.Remove(cthd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CthdExists(string id)
        {
            return _context.Cthds.Any(e => e.MaBanh == id);
        }
    }
}
