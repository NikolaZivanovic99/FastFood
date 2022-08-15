using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
   public class OrderDto
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingCost { get; set; }
        public TimeSpan DurationDelivery { get; set; }
        public DateTime TimeAccepts { get; set; }
        public string Status { get; set; }
        public List<OrderProductDto> Products { get; set; }
        public UserDto Custumer { get; set; }
        public UserDto Deliverer { get; set; }
    }
}
