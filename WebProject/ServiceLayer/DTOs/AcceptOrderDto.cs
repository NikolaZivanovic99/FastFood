using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.DTOs
{
    public class AcceptOrderDto
    {
        public int OrderId { get; set; }
        public int DelivererId { get; set; }
    }
}
