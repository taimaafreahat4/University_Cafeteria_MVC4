
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DatabaseFirst.Data;

namespace DatabaseFirst.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public ProductsController(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToListAsync();

            return View(products);
        }
    }
}
