using System.Linq;
using System.Web.Mvc;
using Ecofriendlyproductmarketplaceproject.Data;

namespace Ecofriendlyproductmarketplaceproject.Controllers
{
     // ✅ Allow only Admins
    public class AdminController : Controller
    {
        private MarketplaceDbContext db = new MarketplaceDbContext();

        public ActionResult PendingProducts()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account"); // ✅ Redirect non-admins to login
            }

            var pendingProducts = db.Products.Where(p => !p.IsApproved).ToList();
            return View(pendingProducts);
        }
    
       // ✅ Approve Product
        public ActionResult Approve(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                product.IsApproved = true;
                db.SaveChanges();
                TempData["Message"] = "Product approved successfully!";
            }
            return RedirectToAction("PendingProducts");
        }

        // ✅ Reject Product
        public ActionResult Reject(int id)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["Message"] = "Product rejected!";
            }
            return RedirectToAction("PendingProducts");
        }
    }
}
