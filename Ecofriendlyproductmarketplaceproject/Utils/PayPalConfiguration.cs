using PayPal.Api;
using System.Collections.Generic;

namespace Ecofriendlyproductmarketplaceproject.Utils
{
    public static class PayPalConfiguration
    {
        public static APIContext GetAPIContext()
        {
            var config = GetConfig();
            string accessToken = new OAuthTokenCredential(config["clientId"], config["clientSecret"]).GetAccessToken();
            return new APIContext(accessToken);
        }

        public static Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>
            {
                { "clientId", "AQtx6TK6vvbm7fREdOarhLlA3_vvWkjPfd5t3QRg8VPvQKDmSH35k1hdH0kAQ_g74jqpuom5hes3qPJy" },
                { "clientSecret", "EOGtiuSkgt6X9cUjPbzKrIi6z_c-_UQrBBsWxTKkLk-R2l8rcm4IhkmPmqKwV1ydTSjPOQObkw4Zsyx6" },
                { "mode", "sandbox" } // Change to "live" for production
            };
        }
    }
}
