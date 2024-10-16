using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace BackendNETAPI.Model
{
    public class Products
    {
        [Key] // This attribute specifies that this property is the primary key
        public int Id { get; set; }
        public string ProductId { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string Category { get; set; } = "";
        public int Price { get; set; }
        public string ImagePath { get; set; } = "";
        public int AlertQty { get; set; } 
        public int Quantity { get; set; }
        public string InventoryStatus { get; set; } = "";



    }
}
