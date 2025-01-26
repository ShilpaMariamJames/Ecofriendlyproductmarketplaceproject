using System;

namespace Ecofriendlyproductmarketplaceproject.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentStatus { get; set; }

        // Navigation properties
        public virtual Order Order { get; set; }
        public virtual User User { get; set; }
    }
}
