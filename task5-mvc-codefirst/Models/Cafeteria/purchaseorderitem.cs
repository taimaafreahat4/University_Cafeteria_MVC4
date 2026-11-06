
namespace task5_mvc_codefirst.Models.Cafeteria
{
    public class purchaseorderitem
    {
        public int Id { get; set; }
        public int quantity { get; set; }
        public decimal unitprice { get; set; }

        public int PurchaseOrderId { get; set; }
        public purchaseorder PurchaseOrder { get; set; } = null!;

        public int ProductId { get; set; }
        public product Product { get; set; } = null!;
    }
}
