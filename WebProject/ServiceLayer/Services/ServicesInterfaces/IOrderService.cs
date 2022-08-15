using DataLayer.Models;
using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.ServicesInterfaces
{
    public interface IOrderService
    {
        List<OrderDto> GetOrders();
        NewOrderDto AddNew(NewOrderDto newOrder);
        List<Order> GetUserOrder(int userId);
        OrderDto AcceptOrder(AcceptOrderDto order);
        List<OrderDto> GetNewOrder();
        OrderDto EndOrder(int id);
    }
}
