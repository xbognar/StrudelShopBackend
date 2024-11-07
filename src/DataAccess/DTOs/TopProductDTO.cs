using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
	public class TopProductDTO
	{
		public int ProductId { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string MainImageURL { get; set; }
	}
}
