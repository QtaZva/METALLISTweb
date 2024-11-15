using METALLIST.Models;
using Microsoft.EntityFrameworkCore;

namespace METALLIST
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Material> Materials { get; set; }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка связи "Один ко многим" между Order и Customer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            // Настройка связи "Один ко многим" между Order и Material
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Material)
                .WithMany(m => m.Orders)
                .HasForeignKey(o => o.MaterialsId);
        }
    }
}
