using System;
using System.Linq;
using System.Web.Mvc;
using PayPal.Api;
using Ecofriendlyproductmarketplaceproject.Utils;
using Ecofriendlyproductmarketplaceproject.Data;
using System.Collections.Generic;
using Ecofriendlyproductmarketplaceproject.Models;

namespace Ecofriendlyproductmarketplaceproject.Controllers
{
    public class PaymentController : Controller
    {

        // ✅ POST: /Payment/CreatePayment
        [HttpPost]
        public ActionResult CreatePayment(int orderId)
        {
            using (var db = new MarketplaceDbContext())
            {
                var order = db.Orders.Find(orderId);
                if (order == null)
                {
                    TempData["Error"] = "Order not found!";
                    return RedirectToAction("Checkout", "Payment");
                }

                var cartItems = db.OrderItems.Where(oi => oi.OrderId == orderId).ToList();
                if (!cartItems.Any())
                {
                    TempData["Error"] = "No items found in this order!";
                    return RedirectToAction("Checkout", "Payment");
                }

                var apiContext = PayPalConfiguration.GetAPIContext();
                var transactionItems = cartItems.Select(item => new Item()
                {
                    name = item.Product.Name,
                    currency = "USD",
                    price = item.Price.ToString("F2", System.Globalization.CultureInfo.InvariantCulture),
                    quantity = item.Quantity.ToString()
                }).ToList();

                var totalAmount = order.TotalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);

                var payment = new PayPal.Api.Payment()
                {
                    intent = "sale",
                    payer = new Payer() { payment_method = "paypal" },
                    transactions = new List<Transaction>()
            {
                new Transaction()
                {
                    item_list = new ItemList() { items = transactionItems },
                    amount = new Amount() { currency = "USD", total = totalAmount }
                }
            },
                    redirect_urls = new RedirectUrls()
                    {
                        return_url = Url.Action("ExecutePayment", "Payment", new { orderId }, Request.Url.Scheme),
                        cancel_url = Url.Action("Cancel", "Payment", null, Request.Url.Scheme)
                    }
                };

                var createdPayment = payment.Create(apiContext);
                var approvalUrl = createdPayment.links.FirstOrDefault(l => l.rel == "approval_url")?.href;

                if (string.IsNullOrEmpty(approvalUrl))
                {
                    TempData["Error"] = "Failed to generate PayPal approval URL.";
                    return RedirectToAction("Checkout", "Payment");
                }

                return Redirect(approvalUrl); // ✅ This sends the user to PayPal
            }
        }

        // ✅ Execute PayPal Payment
        public ActionResult ExecutePayment(int orderId, string paymentId, string token, string PayerID)
        {
            try
            {
                var apiContext = PayPalConfiguration.GetAPIContext();
                var paymentExecution = new PaymentExecution() { payer_id = PayerID };
                var payment = new PayPal.Api.Payment() { id = paymentId };
                var executedPayment = payment.Execute(apiContext, paymentExecution);

                if (executedPayment.state.ToLower() == "approved")
                {
                    using (var db = new MarketplaceDbContext())
                    {
                        var order = db.Orders.Find(orderId);
                        if (order == null)
                        {
                            TempData["Error"] = "Order not found!";
                            return RedirectToAction("Cart", "Product");
                        }

                        var newPayment = new PaymentRecord
                        {
                            OrderId = orderId,
                            PaymentDate = DateTime.Now,
                            Amount = decimal.Parse(order.TotalPrice.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)),
                            PaymentStatus = "Completed"
                        };

                        db.Payments.Add(newPayment);
                        db.SaveChanges();
                    }

                    TempData["Message"] = "Payment successful!";
                    return RedirectToAction("OrderConfirmation", "Product", new { orderId });
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error executing PayPal payment: " + ex.Message;
                return View("Error");
            }
        }

        // ✅ Payment Cancellation
        public ActionResult Cancel()
        {
            TempData["Message"] = "Payment was cancelled!";
            return RedirectToAction("Cart", "Product");
        }

        // ✅ Checkout Process (Ensure Order Exists)
        public ActionResult Checkout(int? orderId)
        {
            var cart = Session["Cart"] as List<CartItem>;

            if (cart == null || cart.Count == 0)
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Browse", "Product");
            }

            if (!orderId.HasValue)
            {
                orderId = GenerateOrderId(cart); // Ensure order exists
            }

            using (var db = new MarketplaceDbContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == orderId.Value);
                if (order == null)
                {
                    return HttpNotFound();
                }

                ViewBag.OrderId = order.Id;
                ViewBag.TotalAmount = order.TotalPrice.ToString("C");
                return View(cart); // ✅ Pass cart data to the View
            }
        }

        // ✅ Generate Order ID if missing
        private int GenerateOrderId(List<CartItem> cart)
        {
            using (var db = new MarketplaceDbContext())
            {
                var newOrder = new Models.Order()
                {
                    TotalPrice = cart.Sum(item => item.Price * item.Quantity),
                    OrderDate = DateTime.Now
                };
                db.Orders.Add(newOrder);
                db.SaveChanges();
                return newOrder.Id;
            }
        }

        // ✅ Payment Record Model
        internal class PaymentRecord : Models.Payment
        {
            public int OrderId { get; set; }
            public DateTime PaymentDate { get; set; }
            public decimal Amount { get; set; }
            public string PaymentStatus { get; set; }
        }
    }
}



