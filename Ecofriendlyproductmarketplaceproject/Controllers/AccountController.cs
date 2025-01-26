using System.Linq;
using System.Web.Mvc;
using Ecofriendlyproductmarketplaceproject.Data;
using Ecofriendlyproductmarketplaceproject.Models;

namespace Ecofriendlyproductmarketplaceproject.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                using (var db = new MarketplaceDbContext())
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                }
                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            using (var db = new MarketplaceDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["Role"] = user.Role;
                    return RedirectToAction("Index", "Home");
                }
            }
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        // GET: /Account/Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

