using StrudelShop.DataAccess.DataAccess;
using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IntegrationTests.Dependencies
{
	public static class SeedDataHelper
	{
		public static void Seed(ApplicationDbContext db)
		{
			// If already seeded, return
			if (db.Users.Any()) return;

			// 1) Users (Admin + normal user)
			var adminUser = new User
			{
				UserID = 1,
				Username = "admin",
				PasswordHash = "adminPass",
				Role = "Admin",
				FirstName = "System",
				LastName = "Administrator",
				Email = "admin@strudelshop.com",
				PhoneNumber = "123456",
				Address = "Admin Lane"
			};
			var normalUser = new User
			{
				UserID = 2,
				Username = "john",
				PasswordHash = "johnPass",
				Role = "User",
				FirstName = "John",
				LastName = "Doe",
				Email = "john@doe.com",
				PhoneNumber = "654321",
				Address = "Doe Street 42"
			};
			db.Users.AddRange(adminUser, normalUser);

			// 2) Products
			var appleProd = new Product
			{
				ProductID = 10,
				Name = "Apple Strudel",
				Description = "Delicious apple pastry",
				Price = 5.99m,
				ImageURL = "apple.jpg",
				ProductImages = new List<ProductImage>()
			};
			var cherryProd = new Product
			{
				ProductID = 11,
				Name = "Cherry Strudel",
				Description = "Cherry-filled pastry",
				Price = 6.99m,
				ImageURL = "cherry.jpg",
				ProductImages = new List<ProductImage>()
			};
			db.Products.AddRange(appleProd, cherryProd);

			// 3) Product Images
			db.ProductImages.Add(new ProductImage
			{
				ImageID = 100,
				ProductID = 10,
				ImageURL = "apple_strudel_1.jpg"
			});
			db.ProductImages.Add(new ProductImage
			{
				ImageID = 101,
				ProductID = 11,
				ImageURL = "cherry_strudel_1.jpg"
			});

			// 4) Orders
			var order1 = new Order
			{
				OrderID = 1000,
				UserID = normalUser.UserID,
				OrderDate = DateTime.UtcNow,
				DeliveryDate = DateTime.UtcNow.AddDays(2),
				TotalAmount = 12.98m,
				PaymentStatus = "Paid",
				OrderItems = new List<OrderItem>()
			};
			var order2 = new Order
			{
				OrderID = 1001,
				UserID = normalUser.UserID,
				OrderDate = DateTime.UtcNow,
				DeliveryDate = DateTime.UtcNow.AddDays(3),
				TotalAmount = 5.99m,
				PaymentStatus = "Pending",
				OrderItems = new List<OrderItem>()
			};
			db.Orders.AddRange(order1, order2);

			// 5) Order Items
			db.OrderItems.Add(new OrderItem
			{
				OrderItemID = 500,
				OrderID = 1000,
				ProductID = 10,
				Quantity = 2,
				Price = 5.99m
			});
			db.OrderItems.Add(new OrderItem
			{
				OrderItemID = 501,
				OrderID = 1000,
				ProductID = 11,
				Quantity = 1,
				Price = 6.99m
			});
			db.OrderItems.Add(new OrderItem
			{
				OrderItemID = 502,
				OrderID = 1001,
				ProductID = 10,
				Quantity = 1,
				Price = 5.99m
			});

			db.SaveChanges();
		}
	}
}
