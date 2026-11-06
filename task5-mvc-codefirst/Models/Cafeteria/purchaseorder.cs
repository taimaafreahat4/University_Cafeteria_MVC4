
namespace task5_mvc_codefirst.Models.Cafeteria
{
    public class purchaseorder
    {
        public int Id { get; set; }
        public DateTime orderdate { get; set; }
        public string status { get; set; } = "";

        public int SupplierId { get; set; }
        public supplier Supplier { get; set; } = null!;

        public ICollection<purchaseorderitem> purchaseorderitems
        { get; set; } = new List<purchaseorderitem>();
    }
}
