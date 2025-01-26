using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ecofriendlyproductmarketplaceproject.Data;
using Ecofriendlyproductmarketplaceproject.Models;

namespace Ecofriendlyproductmarketplaceproject.Controllers
{
    public class ProductController : Controller
    {
        // GET: /Product/Add
        public ActionResult Add()
        {
            return View();
        }

        // POST: /Product/Add
        [HttpPost]
        public ActionResult Add(Product product, HttpPostedFileBase imageFile)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new MarketplaceDbContext())
            {
                int userId = Convert.ToInt32(Session["UserId"]);

                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                product.SellerId = userId;

                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    // Generate a unique file name
                    string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(imageFile.FileName);
                    string filePath = Server.MapPath("~/Uploads/" + fileName);

                    // Save the file to the server
                    imageFile.SaveAs(filePath);

                    // Store the file path in the database
                    product.ImagePath = "/Uploads/" + fileName;
                }
                else
                {
                    // Handle the case where no image is provided
                    product.ImagePath = null;
                }

                if (ModelState.IsValid)
                {
                    db.Products.Add(product);
                    db.SaveChanges();
                    return RedirectToAction("MyProducts");
                }
            }

            return View(product);
        }



        // GET: /Product/MyProducts
        public ActionResult MyProducts()
        {
            int sellerId = Convert.ToInt32(Session["UserId"]);
            using (var db = new MarketplaceDbContext())
            {
                var products = db.Products.Where(p => p.SellerId == sellerId).ToList();
                return View(products);
            }
        }

        public ActionResult Browse()
        {
            using (var db = new MarketplaceDbContext())
            {
                var products = db.Products.ToList();
                return View(products);
            }
        }

        // POST: /Product/Buy/{id}
        [HttpPost]
        public ActionResult Buy(int id)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            using (var db = new MarketplaceDbContext())
            {
                var product = db.Products.FirstOrDefault(p => p.Id == id);
                if (product == null)
                {
                    return HttpNotFound();
                }

                // Create a new order (Pending status)
                var order = new Order
                {
                    UserId = (int)Session["UserId"],
                    ProductId = product.Id,
                    Quantity = 1,
                    TotalPrice = product.Price,
                    OrderDate = DateTime.Now
                };

                db.Orders.Add(order);
                db.SaveChanges();

                // Redirect to the payment/checkout page
                return RedirectToAction("Checkout", "Payment", new { orderId = order.Id });
            }
        }


        // GET: /Product/OrderConfirmation
        public ActionResult OrderConfirmation(int orderId)
        {
            using (var db = new MarketplaceDbContext())
            {
                var order = db.Orders.Include("Product").FirstOrDefault(o => o.Id == orderId);
                var payment = db.Payments.FirstOrDefault(p => p.OrderId == orderId);

                if (order == null)
                {
                    return HttpNotFound();
                }

                ViewBag.PaymentStatus = payment?.PaymentStatus ?? "Pending";
                ViewBag.OrderId = order.Id;
                ViewBag.TotalAmount = order.TotalPrice;
                ViewBag.ProductName = order.Product.Name;

                return View();
            }
        }


        public ActionResult MyOrders()
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = (int)Session["UserId"];
            using (var db = new MarketplaceDbContext())
            {
                var orders = db.Orders
                    .Where(o => o.UserId == userId)
                    .Include("Product") // Include product details
                    .ToList();

                return View(orders); // Pass the list of orders to the view
            }
        }
    }
}





    


