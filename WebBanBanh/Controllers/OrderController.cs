using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebBanBanh.Models.VNPAY;

namespace WebBanBanh.Controllers
{
    public class OrderController : Controller
    {
        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult OrderSuccess()
        {
            try
            {
                var query = Request.Query;
                var vnp_ResponseCode = query["vnp_ResponseCode"].ToString();
                var vnp_TxnRef = query["vnp_TxnRef"].ToString();
                var vnp_Amount = query["vnp_Amount"].ToString();
                var vnp_OrderInfo = query["vnp_OrderInfo"].ToString();
                var vnp_TransactionNo = query["vnp_TransactionNo"].ToString();
                var vnp_BankCode = query["vnp_BankCode"].ToString();
                var vnp_CardType = query["vnp_CardType"].ToString();

                var paymentResponse = new PaymentResponseModel
                {
                    OrderId = vnp_TxnRef,
                    TransactionId = vnp_TransactionNo,
                    OrderDescription = vnp_OrderInfo,
                    VnPayResponseCode = vnp_ResponseCode,
                    PaymentMethod = "VNPAY",
                    BankCode = vnp_BankCode,
                    CardType = vnp_CardType
                };

                return View(paymentResponse); // ✅ Phải trả về view
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý OrderSuccess");

                return View(new PaymentResponseModel
                {
                    VnPayResponseCode = "99",
                    OrderDescription = "Lỗi xử lý giao dịch"
                });
            }
        }
    }
}
