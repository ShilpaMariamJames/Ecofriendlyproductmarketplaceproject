using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecofriendlyproductmarketplaceproject.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}
