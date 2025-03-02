using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                    string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(imageFile.FileName);
                    string filePath = Server.MapPath("~/Uploads/" + fileName);
                    imageFile.SaveAs(filePath);
                    product.ImagePath = "/Uploads/" + fileName;
                }
                else
                {
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

        // ✅ Display all products for browsing
        public ActionResult Browse()
        {
            using (var db = new MarketplaceDbContext())
            {
                var products = db.Products.Where(p => p.IsApproved).ToList(); // Show only approved products
                return View(products);
            }
        }

        // ✅ Add Product to Cart (Updated Code)
        [HttpPost]
        public ActionResult AddToCart(int[] selectedProducts, Dictionary<string, string> quantities)
        {
            if (selectedProducts == null || selectedProducts.Length == 0)
            {
                TempData["Error"] = "No products selected!";
                return RedirectToAction("Browse", "Product");
            }

            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();

            using (var db = new MarketplaceDbContext())
            {
                foreach (var productId in selectedProducts)
                {
                    var product = db.Products.Find(productId);
                    if (product != null)
                    {
                        // Parse quantity from the dictionary
                        int quantity = quantities.ContainsKey(productId.ToString()) ? int.Parse(quantities[productId.ToString()]) : 1;

                        var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
                        if (existingItem != null)
                        {
                            existingItem.Quantity += quantity;
                        }
                        else
                        {
                            cart.Add(new CartItem
                            {
                                ProductId = product.Id,
                                ProductName = product.Name,
                                Price = product.Price,
                                Quantity = quantity,
                                ImagePath = product.ImagePath
                            });
                        }
                    }
                }
            }

            Session["Cart"] = cart;
            TempData["Message"] = "Products added to cart!";
            return RedirectToAction("Cart", "Product");
        }

        // ✅ Display Cart
        public ActionResult Cart()
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            System.Diagnostics.Debug.WriteLine($"Cart Session Count: {cart.Count}");
            return View(cart);
        }

        // ✅ Remove Item from Cart
        public ActionResult RemoveFromCart(int productId)
        {
            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);
            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
            }

            Session["Cart"] = cart;
            return RedirectToAction("Cart");
        }

        // ✅ Checkout & Order Creation
        public ActionResult Checkout()
        {
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to proceed to checkout.";
                return RedirectToAction("Login", "Account");
            }

            var cart = Session["Cart"] as List<CartItem>;

            if (cart == null || !cart.Any())
            {
                TempData["ErrorMessage"] = "Your cart is empty.";
                return RedirectToAction("Cart");
            }

            using (var db = new MarketplaceDbContext())
            {
                var userId = (int)Session["UserId"];

                var order = new Order
                {
                    UserId = userId,
                    TotalPrice = cart.Sum(c => c.Price * c.Quantity),
                    OrderDate = DateTime.Now
                };

                db.Orders.Add(order);
                db.SaveChanges();

                foreach (var cartItem in cart)
                {
                    var orderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price
                    };
                    db.OrderItems.Add(orderItem);
                }

                db.SaveChanges();

                Session["OrderId"] = order.Id; // ✅ Store order ID for payment redirection
                Session["Cart"] = null; // ✅ Clear cart after placing order

                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }
        }


        // ✅ Order Confirmation Page
        public ActionResult OrderConfirmation(int orderId)
        {
            using (var db = new MarketplaceDbContext())
            {
                var order = db.Orders.Include(o => o.OrderItems.Select(oi => oi.Product))
                                     .FirstOrDefault(o => o.Id == orderId);
                if (order == null)
                {
                    return HttpNotFound();
                }

                // Fetch the payment record for the order
                var payment = db.Payments.FirstOrDefault(p => p.OrderId == orderId);

                // Pass payment status to the view
                ViewBag.PaymentStatus = payment != null ? payment.PaymentStatus : "Pending";

                return View(order);
            }
        }

        // ✅ Display User Orders
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
                    .Include(o => o.OrderItems.Select(oi => oi.Product))
                    .ToList();
                return View(orders);
            }
        }
    }
}








