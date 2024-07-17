using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StrudelShop.DataAccess.DTOs
{
	public class OrderDTO
	{

		public int OrderId { get; set; }
		
		public int CustomerId { get; set; }
		
		public DateTime OrderDate { get; set; }
		
		public DateTime DeliveryDate { get; set; }
		
		public decimal TotalAmount { get; set; }
		
		public string PaymentStatus { get; set; }
		
		public List<OrderItemDTO> OrderItems { get; set; }

	}

}
