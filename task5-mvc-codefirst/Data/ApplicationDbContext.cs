
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using task5_mvc_codefirst.Models.Cafeteria;

namespace task5_mvc_codefirst.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<product> products { get; set; }
        public DbSet<supplier> suppliers { get; set; }
        public DbSet<purchaseorder> purchaseorders { get; set; }
        public DbSet<purchaseorderitem> purchaseorderitems { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<product>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.products)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<purchaseorder>()
                .HasOne(p => p.Supplier)
                .WithMany(s => s.purchaseorders)
                .HasForeignKey(p => p.SupplierId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<purchaseorderitem>()
                .HasOne(i => i.PurchaseOrder)
                .WithMany(p => p.purchaseorderitems)
                .HasForeignKey(i => i.PurchaseOrderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<purchaseorderitem>()
                .HasOne(i => i.Product)
                .WithMany(p => p.purchaseorderitems)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<product>()
                .Property(p => p.price)
                .HasPrecision(18, 2);

            builder.Entity<purchaseorderitem>()
                .Property(i => i.unitprice)
                .HasPrecision(18, 2);
        }
    }
}
