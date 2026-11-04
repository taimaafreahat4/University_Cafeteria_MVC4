
using System.Collections.Generic;

namespace UniversityCafeteria.Models
{
    public class SupplyRequestSummary
    {
        public List<SupplyItem> Items { get; set; } = new();

        public decimal Subtotal { get; set; }

        public decimal PreparationFee { get; set; }

        public decimal Discount { get; set; }

        public decimal FinalTotal { get; set; }

        public string Priority { get; set; } = "Normal";
    }
}
