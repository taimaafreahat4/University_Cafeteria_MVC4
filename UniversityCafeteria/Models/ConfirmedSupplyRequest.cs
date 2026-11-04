
namespace UniversityCafeteria.Models
{
    public class ConfirmedSupplyRequest
    {
        public int RequestID { get; set; }

        public string EmployeeKey { get; set; } = "";

        public int NumberOfProducts { get; set; }

        public decimal Subtotal { get; set; }

        public decimal PreparationFee { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalTotal { get; set; }

        public string Status { get; set; } = "Confirmed";
    }
}
