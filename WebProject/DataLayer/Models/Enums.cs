using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public enum OrderStatus
    {
        AwaitingDelivery,
        IsDelivered,
        Delivered
    };

    public enum UserType
    {
        Admin,
        Consumer,
        Deliverer
    }
}
