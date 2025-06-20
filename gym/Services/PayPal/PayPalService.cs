using gym.Models;
using gym.Models.PayPal;
using gym.Services.PayPal;
using Microsoft.Extensions.Configuration;
using PayPalCheckoutSdk.Orders;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace gym.Services.PayPal
{
    public class PayPalService
    {
        private readonly IConfiguration _configuration;

        public PayPalService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreatePaymentUrl(PaymentInformationModel model, string returnUrl, string cancelUrl)
        {
            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");

            request.RequestBody(new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
                {
                    new PurchaseUnitRequest
                    {
                        AmountWithBreakdown = new AmountWithBreakdown
                        {
                            CurrencyCode = "USD",
                            Value = model.Amount.ToString("F2", CultureInfo.InvariantCulture)
                        },
                        Description = model.Description
                    }
                },
                ApplicationContext = new ApplicationContext
                {
                    ReturnUrl = returnUrl,
                    CancelUrl = cancelUrl
                }
            });

            var response = await PayPalClientFactory.Client(_configuration).Execute(request);
            var result = response.Result<Order>();

            return result.Links.FirstOrDefault(l => l.Rel == "approve")?.Href;
        }
    }
}
