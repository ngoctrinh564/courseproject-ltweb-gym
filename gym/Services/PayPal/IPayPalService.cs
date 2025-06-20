using gym.Models.PayPal;

namespace gym.Services.PayPal
{
    public interface IPayPalService
    {
        Task<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext httpContext);
        Task<string> ExecutePayment(HttpRequest request);
    }
}
