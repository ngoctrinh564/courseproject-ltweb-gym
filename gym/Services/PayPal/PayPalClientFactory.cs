using PayPalCheckoutSdk.Core;
using BraintreeHttp;
using Microsoft.Extensions.Configuration;
using HttpClient = BraintreeHttp.HttpClient;

namespace gym.Services.PayPal
{
    public static class PayPalClientFactory
    {
        public static PayPalEnvironment Environment(IConfiguration config)
        {
            return new SandboxEnvironment(
                config["Paypal:ClientId"],
                config["Paypal:SecretKey"]);
        }

        public static HttpClient Client(IConfiguration config)
        {
            return new PayPalHttpClient(Environment(config));
        }
    }
}
