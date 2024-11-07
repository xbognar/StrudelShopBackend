using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Models
{
	public class Order
	{
		public int OrderID { get; set; }
		public int UserID { get; set; }
		public DateTime OrderDate { get; set; }
		public DateTime DeliveryDate { get; set; }
		public decimal TotalAmount { get; set; }
		public string PaymentStatus { get; set; }

		// Navigation properties
		public User User { get; set; }
		public ICollection<OrderItem> OrderItems { get; set; }
	}

}
