using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace StrudelShop.DataAccess.DataAccess
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

		public DbSet<User> Users { get; set; }
		public DbSet<Product> Products { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<Order> Orders { get; set; }
		public DbSet<OrderItem> OrderItems { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>()
				.HasMany(p => p.ProductImages)
				.WithOne(pi => pi.Product)
				.HasForeignKey(pi => pi.ProductID);

			modelBuilder.Entity<Order>()
				.HasMany(o => o.OrderItems)
				.WithOne(oi => oi.Order)
				.HasForeignKey(oi => oi.OrderID);

			modelBuilder.Entity<User>()
				.HasMany(u => u.Orders)
				.WithOne(o => o.User)
				.HasForeignKey(o => o.UserID);

			base.OnModelCreating(modelBuilder);
		}
	}
}
