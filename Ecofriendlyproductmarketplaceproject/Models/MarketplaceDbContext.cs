using System.Data.Entity;
using Ecofriendlyproductmarketplaceproject.Models;

namespace Ecofriendlyproductmarketplaceproject.Data
{
    public class MarketplaceDbContext : DbContext
    {
        public MarketplaceDbContext() : base("EcoFriendlyDb") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }





    }
}
