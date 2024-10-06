using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace BackendNETAPI.Model
{
    public class Products
    {
        [Key] // This attribute specifies that this property is the primary key
        public int Id { get; set; }
        public string EmpID { get; set; } = "";
        public string EmpName { get; set; } = "";
        public string Address { get; set; } = "";
        public string ContactNo { get; set; } = "";
        public string ImagePath { get; set; } // Use string? for nullable



    }
}
