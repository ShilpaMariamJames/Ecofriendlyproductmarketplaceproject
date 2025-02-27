using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ecofriendlyproductmarketplaceproject.Models
{
    public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }  // Foreign Key to Users Table

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    public decimal TotalPrice { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; }
}

}
