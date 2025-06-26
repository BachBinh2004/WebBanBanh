using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebBanBanh.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace WebBanBanh.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly WebBanBanhContext _context;

        public CartItemsController(WebBanBanhContext context)
        {
            _context = context;
        }
        const string CARTKEY = "cart";
        List<CartItem> GetCart()
        {
            var session = HttpContext.Session.GetString(CARTKEY);
            return string.IsNullOrEmpty(session)
            ? new List<CartItem>()
            : JsonConvert.DeserializeObject<List<CartItem>>(session) ??

            new();
        }
        void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString(CARTKEY,

            JsonConvert.SerializeObject(cart));
        }
        // GET: CartController
        public ActionResult Index()
        {
            var cart = GetCart();
            ViewBag.Total = cart.Sum(i => i.Price * i.Quantity);
            return View(cart);
        }
        [HttpGet]
        [HttpGet]
        public IActionResult AddToCart(string id, int soLuong = 1) // Mặc định là 1
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Thiếu mã bánh!" });
            }

            var product = _context.Banhs.Find(id);
            if (product == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm!" });
            }

            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.Id == id);

            if (item != null)
                item.Quantity += soLuong;
            else
                cart.Add(new CartItem
                {
                    Id = product.MaBanh,
                    Name = product.TenBanh,
                    Price = product.Gia,
                    Quantity = soLuong
                });

            SaveCart(cart);

            return Json(new { success = true, totalQuantity = cart.Sum(i => i.Quantity) });
        }

        public class CartItemDto
        {
            public string Id { get; set; }
            public int Quantity { get; set; }
        }
        [HttpPost]
        public IActionResult Checkout(List<CartItemDto> cartItems)
        {
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "CartItem"); // Quay lại giỏ hàng nếu không có sản phẩm
            }

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
                HinhThuc = "Đặt bánh",
                MaKh = "KH1",
                MaNv = "NV1"
            };

            // Lấy danh sách bánh trong giỏ hàng
            var banhIds = cartItems.Select(i => i.Id).ToList();
            var banhList = _context.Banhs.Where(b => banhIds.Contains(b.MaBanh)).ToList();

            var cartItemsProcessed = cartItems.Select(item =>
            {
                var banh = banhList.FirstOrDefault(b => b.MaBanh == item.Id);
                return new CartItem
                {
                    Id = banh?.MaBanh ?? "",
                    Name = banh?.TenBanh ?? "Không xác định",
                    ImageUrl = banh?.Hinhanh ?? "",
                    Price = banh?.Gia ?? 0,
                    Quantity = item.Quantity
                };
            }).ToList();

            // Lưu giỏ hàng vào Session (dùng JSON để serialize)
            HttpContext.Session.SetString("CartItems", JsonConvert.SerializeObject(cartItemsProcessed));

            // Lưu mã hóa đơn vào Session
            HttpContext.Session.SetString("CurrentOrder", JsonConvert.SerializeObject(hd));

            return RedirectToAction("Checkout");
        }

        // GET: CheckoutView (View hiển thị thông tin hóa đơn)
        public IActionResult Checkout()
        {
            // Lấy dữ liệu từ Session
            var cartItemsJson = HttpContext.Session.GetString("CartItems");
            var cartItems = string.IsNullOrEmpty(cartItemsJson)
                ? new List<CartItemDto>()
                : JsonConvert.DeserializeObject<List<CartItemDto>>(cartItemsJson);

            var orderJson = HttpContext.Session.GetString("CurrentOrder");
            var order = string.IsNullOrEmpty(orderJson)
                ? new HoaDon()
                : JsonConvert.DeserializeObject<HoaDon>(orderJson);
            
            // Đưa dữ liệu ra View
            ViewBag.CartItems = cartItems;
            return View(order);
        }

        public IActionResult Remove(string id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.Id == id);
            if (item != null) cart.Remove(item);
            SaveCart(cart);
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult UpdateQuantity(string productId, int quantity)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(p => p.Id == productId);
            if (item != null && quantity > 0)
            {
                item.Quantity = quantity;
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Clear()
        {
            SaveCart(new List<CartItem>());
            return RedirectToAction("Index");
        }
        // GET: CartItems/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cartItem == null)
            {
                return NotFound();
            }

            return View(cartItem);
        }

        // GET: CartItems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CartItems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Price,ImageUrl,Quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cartItem);
        }

        // GET: CartItems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cartItem = await _context.CartItem.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }
            return View(cartItem);
        }


    }
}
