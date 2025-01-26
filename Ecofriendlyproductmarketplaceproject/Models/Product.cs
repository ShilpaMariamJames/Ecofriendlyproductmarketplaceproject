using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecofriendlyproductmarketplaceproject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int SellerId { get; set; }
        public string ImagePath { get; set; }
    }
}