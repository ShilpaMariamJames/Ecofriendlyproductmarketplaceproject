using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ecofriendlyproductmarketplaceproject.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public string ImagePath { get; set; }  // Ensure this property exists
        public string ProductName { get; set; } // ✅ Add this property
    }

}


