using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DTOs
{
	public class OrderDetailsDTO
	{
		
		public int OrderItemId { get; set; }
		
		public string ProductName { get; set; }
		
		public int Quantity { get; set; }
		
		public decimal Price { get; set; }
		
		public decimal TotalPrice { get; set; }
		
		public DateTime OrderDate { get; set; }
		
		public DateTime DeliveryDate { get; set; }
		
		public decimal OrderTotalAmount { get; set; }
		
		public string PaymentStatus { get; set; }
		
		public string CustomerFirstName { get; set; }
		
		public string CustomerLastName { get; set; }
		
		public string CustomerEmail { get; set; }
		
		public string CustomerPhoneNumber { get; set; }
		
		public string CustomerAddress { get; set; }
	
	}

}
