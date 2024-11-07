using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public class Product
	{
		public int ProductID { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public decimal Price { get; set; }
		public string ImageURL { get; set; }  // Main catalog image

		// Navigation property for additional images
		public ICollection<ProductImage> ProductImages { get; set; }
	}

}
