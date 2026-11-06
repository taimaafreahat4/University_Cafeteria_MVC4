
namespace task5_mvc_codefirst.Models.Cafeteria
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";

        public ICollection<product> Products { get; set; }
            = new List<product>();
    }
}
