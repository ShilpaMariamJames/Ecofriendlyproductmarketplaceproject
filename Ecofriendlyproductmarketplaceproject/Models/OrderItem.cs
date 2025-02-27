using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecofriendlyproductmarketplaceproject.Models
{
   
    public class OrderItem
        {
            [Key]
            public int Id { get; set; }

            [Required]
            public int OrderId { get; set; }

            [Required]
            public int ProductId { get; set; }

            [Required]
            public int Quantity { get; set; }

            [Required]
            [Column(TypeName = "decimal")] // Fix for the decimal error
            public decimal Price { get; set; }

            // Navigation properties
            [ForeignKey("OrderId")]
            public virtual Order Order { get; set; }

            [ForeignKey("ProductId")]
            public virtual Product Product { get; set; }
        }
    }

