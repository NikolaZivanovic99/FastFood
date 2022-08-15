using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
   public class NewOrderDto
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string Comment { get; set; }
        public decimal Price { get; set; }
        public decimal ShippingCost { get; set; }
        public List<OrderProductDto> Products { get; set; }
        public int CustumerId { get; set; }
    }
}
