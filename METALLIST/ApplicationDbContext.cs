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
        public DbSet<OrderMaterial> OrderMaterials { get; set; }
        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка композитного ключа для OrderMaterial
            modelBuilder.Entity<OrderMaterial>()
                .HasKey(om => new { om.OrderId, om.MaterialId });

            // Настройка каскадного удаления для OrderMaterial -> Order
            modelBuilder.Entity<OrderMaterial>()
                .HasOne(om => om.Order)
                .WithMany(o => o.OrderMaterials)
                .HasForeignKey(om => om.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка каскадного удаления для OrderMaterial -> Material
            modelBuilder.Entity<OrderMaterial>()
                .HasOne(om => om.Material)
                .WithMany(m => m.OrderMaterials)
                .HasForeignKey(om => om.MaterialId)
                .OnDelete(DeleteBehavior.Cascade);

            // Настройка каскадного удаления для Order -> Customer
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Если есть другие связи, добавьте их здесь
        }
    }
}
