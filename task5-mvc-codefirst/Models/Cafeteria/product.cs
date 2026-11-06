
namespace task5_mvc_codefirst.Models.Cafeteria
{
    public class product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal price { get; set; }
        public int quantity { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int SupplierId { get; set; }
        public supplier Supplier { get; set; } = null!;

        public ICollection<purchaseorderitem> purchaseorderitems
        { get; set; } = new List<purchaseorderitem>();
    }
}
