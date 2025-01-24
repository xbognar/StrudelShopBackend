using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace StrudelShop.DataAccess.DataAccess
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public virtual DbSet<User> Users { get; set; }
		public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<Product> Products { get; set; }
		public virtual DbSet<OrderItem> OrderItems { get; set; }
		public virtual DbSet<ProductImage> ProductImages { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<ProductImage>()
				.HasKey(pi => pi.ImageID);

			modelBuilder.Entity<Product>()
				.HasMany(p => p.ProductImages)
				.WithOne(pi => pi.Product)
				.HasForeignKey(pi => pi.ProductID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Order>()
				.HasMany(o => o.OrderItems)
				.WithOne(oi => oi.Order)
				.HasForeignKey(oi => oi.OrderID)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<User>()
				.HasMany(u => u.Orders)
				.WithOne(o => o.User)
				.HasForeignKey(o => o.UserID)
				.OnDelete(DeleteBehavior.Cascade);

			base.OnModelCreating(modelBuilder);
		}
	}
}
