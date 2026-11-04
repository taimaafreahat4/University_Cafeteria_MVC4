
using System.ComponentModel.DataAnnotations;

namespace UniversityCafeteria.Models
{
    public class SupplyItem
    {
        [Required(ErrorMessage = "Product name is required")]
        public string ProductName { get; set; } = "";

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }

        [Range(typeof(decimal), "0.01", "99999999",
            ErrorMessage = "Unit price must be greater than zero")]
        public decimal UnitPrice { get; set; }

        [Required]
        public string Priority { get; set; } = "Normal";

        public decimal TotalCost
        {
            get { return Quantity * UnitPrice; }
        }
    }
}
