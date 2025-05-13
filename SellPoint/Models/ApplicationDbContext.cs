using Microsoft.EntityFrameworkCore;

namespace SellPoint.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .Property(u => u.Role)
                .IsRequired();

            // Product -> Seller (User) ilişkisini kuruyoruz
            modelBuilder.Entity<Product>()
                .HasOne<User>()  // Seller, User tablosuyla ilişkili
                .WithMany()  // Her satıcının birden fazla ürünü olabilir
                .HasForeignKey(p => p.SellerId)  // SellerId, foreign key olarak kullanılıyor
                .OnDelete(DeleteBehavior.Restrict);  // Seller silindiğinde ürünler silinmesin

            // CartItem -> Customer ilişkisi
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Customer)
                .WithMany()
                .HasForeignKey(ci => ci.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // CartItem -> Product ilişkisi
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem -> Order ilişkisi
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderItem -> Product ilişkisi
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // OrderItem -> Seller ilişkisi
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Seller)
                .WithMany()
                .HasForeignKey(oi => oi.SellerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
