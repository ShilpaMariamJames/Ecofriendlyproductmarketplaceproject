using System.Web.Mvc;

namespace Ecofriendlyproductmarketplaceproject.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View(); // This will look for a view in Views/Home/Index.cshtml
        }
    }
}
