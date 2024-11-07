using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
	public class OrderDetailsDTO
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public decimal OrderTotalAmount { get; set; }
		public string PaymentStatus { get; set; }

		// User Information (for Admin views)
		public string CustomerFirstName { get; set; }
		public string CustomerLastName { get; set; }
		public string CustomerEmail { get; set; }
		public string CustomerPhoneNumber { get; set; }
		public string CustomerAddress { get; set; }

		// List of order items
		public List<OrderItemDTO> OrderItems { get; set; }
	}
}
