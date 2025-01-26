using Ecofriendlyproductmarketplaceproject.Data;
using System.Web.Mvc;
using System;
using System.Linq;
using Ecofriendlyproductmarketplaceproject.Models;

public class PaymentController : Controller
{
    // GET: /Payment/Checkout/{orderId}
    public ActionResult Checkout(int orderId)
    {
        using (var db = new MarketplaceDbContext())
        {
            var order = db.Orders.Include("Product").FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            ViewBag.OrderId = order.Id;
            ViewBag.TotalAmount = order.TotalPrice;
            ViewBag.ProductName = order.Product.Name;

            return View();
        }
    }

    // POST: /Payment/Checkout
    [HttpPost]
    public ActionResult Checkout(int orderId, decimal amount)
    {
        if (Session["UserId"] == null)
        {
            return RedirectToAction("Login", "Account");
        }

        using (var db = new MarketplaceDbContext())
        {
            var payment = new Payment
            {
                UserId = (int)Session["UserId"],
                OrderId = orderId,
                PaymentDate = DateTime.Now,
                Amount = amount,
                PaymentStatus = "Success"
            };

            db.Payments.Add(payment);
            db.SaveChanges();

            TempData["Message"] = "Payment successful!";
            return RedirectToAction("OrderConfirmation", "Product", new { orderId });
        }
    }
}
