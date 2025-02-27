using System.Linq;
using System.Web.Mvc;
using Ecofriendlyproductmarketplaceproject.Data;
using Ecofriendlyproductmarketplaceproject.Models;

namespace Ecofriendlyproductmarketplaceproject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var db = new MarketplaceDbContext())
            {
                if (Session["UserId"] != null && Session["UserRole"]?.ToString() == "Buyer")
                {
                    var products = db.Products.ToList();
                    return View(products);
                }
                return View();
            }
        }
    }
}

