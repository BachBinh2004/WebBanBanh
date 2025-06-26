using WebBanBanh.Models.VNPAY;

namespace WebBanBanh.Services.VNPAY
{
    public interface IVnPayService
    {
        string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
        PaymentResponseModel PaymentExecute(IQueryCollection collections);

    }

}
