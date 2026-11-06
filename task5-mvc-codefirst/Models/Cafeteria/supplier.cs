
namespace task5_mvc_codefirst.Models.Cafeteria
{
    public class supplier
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string phone { get; set; } = "";

        public ICollection<product> products { get; set; }
            = new List<product>();

        public ICollection<purchaseorder> purchaseorders { get; set; }
            = new List<purchaseorder>();
    }
}
