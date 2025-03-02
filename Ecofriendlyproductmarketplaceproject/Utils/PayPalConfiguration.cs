using PayPal.Api;
using System.Collections.Generic;
using System.Configuration;

namespace Ecofriendlyproductmarketplaceproject.Utils
{
    public static class PayPalConfiguration
    {
        public static APIContext GetAPIContext()
        {
            string clientId = ConfigurationManager.AppSettings["clientId"];
            string clientSecret = ConfigurationManager.AppSettings["clientSecret"];

            string accessToken = new OAuthTokenCredential(clientId, clientSecret).GetAccessToken();
            APIContext apiContext = new APIContext(accessToken)
            {
                Config = GetConfig()
            };
            return apiContext;
        }

        public static Dictionary<string, string> GetConfig()
        {
            return new Dictionary<string, string>
            {
                { "mode", ConfigurationManager.AppSettings["mode"] } // Fetching mode dynamically
            };
        }
    }
}




