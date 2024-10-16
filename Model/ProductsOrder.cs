using System.ComponentModel.DataAnnotations;

namespace BackendNETAPI.Model
{
    public class ProductsOrder
    {
        [Key] 
        public int OrderId { get; set; }
        public string ProductName { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string ImagePath { get; set; } = "";
        public int UserId { get; set; }

    }
}
