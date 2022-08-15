using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingCost { get; set; }
        public TimeSpan DurationDelivery { get; set; }
        public DateTime TimeAccepts { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderProduct> Products { get; set; }
        public int CustumerId { get; set; }
        public User Custumer { get; set; }
        public int DelivererId { get; set; }
        public User Deliverer { get; set; }


    }
}
