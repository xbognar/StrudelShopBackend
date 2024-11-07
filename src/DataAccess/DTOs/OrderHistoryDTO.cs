using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DTOs
{
	public class OrderHistoryDTO
	{
		public int OrderId { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public decimal TotalAmount { get; set; }
		public string PaymentStatus { get; set; }
	}
}
