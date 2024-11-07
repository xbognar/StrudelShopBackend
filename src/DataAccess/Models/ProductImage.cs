using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public class ProductImage
	{
		public int ImageID { get; set; }
		public int ProductID { get; set; }
		public string ImageURL { get; set; }  // URL for each additional image

		// Navigation property
		public Product Product { get; set; }
	}

}
