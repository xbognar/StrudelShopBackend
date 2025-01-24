using DataAccess.Models;
using StrudelShop.DataAccess.DataAccess;
using System;
using System.Collections.Generic;

namespace IntegrationTests.Dependencies
{
	public static class SeedDataHelper
	{
		public static void Seed(ApplicationDbContext db)
		{
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
				Description = "Cherry pastry",
				Price = 6.99m,
				ImageURL = "cherry.jpg",
				ProductImages = new List<ProductImage>()
			};
			db.Products.AddRange(appleProd, cherryProd);

			var appleImage1 = new ProductImage
			{
				ImageID = 100,
				ProductID = 10,
				ImageURL = "apple_strudel_1.jpg"
			};
			var appleImage2 = new ProductImage
			{
				ImageID = 101,
				ProductID = 10,
				ImageURL = "apple_strudel_2.jpg"
			};
			db.ProductImages.AddRange(appleImage1, appleImage2);

			var order1 = new Order
			{
				OrderID = 1000,
				UserID = 2, 
				TotalAmount = 20.00m,
				PaymentStatus = "Pending"
			};
			var order2 = new Order
			{
				OrderID = 1001,
				UserID = 2,
				TotalAmount = 35.00m,
				PaymentStatus = "Paid"
			};
			db.Orders.AddRange(order1, order2);

			var orderItem1 = new OrderItem
			{
				OrderItemID = 500,
				OrderID = 1000,
				ProductID = 10,
				Quantity = 2,
				Price = 5.99m
			};
			var orderItem2 = new OrderItem
			{
				OrderItemID = 501,
				OrderID = 1000,
				ProductID = 11,
				Quantity = 1,
				Price = 6.99m
			};
			db.OrderItems.AddRange(orderItem1, orderItem2);

			db.SaveChanges();
		}
	}
}
