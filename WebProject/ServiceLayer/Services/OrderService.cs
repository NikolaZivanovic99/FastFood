using AutoMapper;
using DataLayer;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DTOs;
using ServiceLayer.Services.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public OrderService(DataContext dataContext, IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public OrderDto AcceptOrder(AcceptOrderDto order)
        {
            var deliverer = _dataContext.Users.Where(x => x.Id == order.DelivererId).FirstOrDefault();
            var dbOrder = GetOrderById(order.OrderId);
            if (deliverer == null || dbOrder == null) 
            {
                throw new Exception("Bad Request!");
            }
            if (dbOrder.Status != OrderStatus.AwaitingDelivery) 
            {
                throw new Exception("It is not possible to take over the order because it has already been delivered or is being delivered");
            }
            if (deliverer.Verification == false) 
            {
                throw new Exception("User is not verified");
            }
            foreach (var item in deliverer.Delivery) 
            {
                if (item.Status == OrderStatus.Delivered) 
                {
                    throw new Exception($"It is not possible to take over a new order until order {item.Id} " +
                        $"is completed.");
                } 
            }
            int minutes = new Random().Next(15, 70);
            TimeSpan durationDelivery = new TimeSpan(0, minutes, 0);
            DateTime timeAccept = DateTime.Now;

            dbOrder.DurationDelivery = durationDelivery;
            dbOrder.TimeAccepts = timeAccept;
            dbOrder.Status = OrderStatus.Delivered;
            dbOrder.Deliverer = deliverer;
            dbOrder.DelivererId = deliverer.Id;

            deliverer.Delivery.Add(dbOrder);
            _dataContext.Users.Update(deliverer);
            _dataContext.Orders.Update(dbOrder);
            _dataContext.SaveChanges();
            return _mapper.Map<OrderDto>(dbOrder);
        }

        public NewOrderDto AddNew(NewOrderDto newOrder)
        {
            NewOrderDto orderBack = null;
            if (IsOrderValid(newOrder)) 
            {
                Order dbOrder = _mapper.Map<Order>(newOrder);
                var orders = GetUserOrder(newOrder.CustumerId);
                foreach (var item in orders) 
                {
                    if (item.Status == OrderStatus.Delivered || item.Status == OrderStatus.AwaitingDelivery) 
                    {
                        throw new Exception("It is not possible to make the next order until the old one is finished");
                    }
                }

                Order orderAdd = new Order
                {
                    Address = dbOrder.Address,
                    Price = dbOrder.Price,
                    ShippingCost = dbOrder.ShippingCost,
                    Comment = dbOrder.Comment,
                    CustumerId = dbOrder.CustumerId,
                    DelivererId = dbOrder.DelivererId,
                    Products = new List<OrderProduct>()
                };
                foreach (var item in dbOrder.Products) 
                {
                    var product = _dataContext.Products.Find(item.Product.Id);
                    orderAdd.Products.Add(new OrderProduct()
                    {
                        Product = product,
                        Order = orderAdd,
                        Quantity = item.Quantity
                    });
                }
                _dataContext.Orders.Add(orderAdd);
                _dataContext.SaveChanges();
                orderBack = _mapper.Map<NewOrderDto>(dbOrder);
            }

            return orderBack;
        }

        public OrderDto EndOrder(int id)
        {
            var order = GetOrderById(id);
            if (order == null || order.Status != OrderStatus.Delivered) 
            {
                throw new Exception("It is not possible to solve the order");
            }
            DateTime date = DateTime.Now;
            if (order.TimeAccepts.Add(order.DurationDelivery) > date) 
            {
                throw new Exception("It is not possible to complete the delivery before the delivery expires ");
            }
            order.Status = OrderStatus.IsDelivered;
            _dataContext.Orders.Update(order);
            _dataContext.SaveChanges();

            return _mapper.Map<OrderDto>(order);
        }

        public List<OrderDto> GetNewOrder()
        {
            var orders = _dataContext.Orders.Where(x => x.Status == OrderStatus.AwaitingDelivery).Include(x => x.Products).ThenInclude(x => x.Product).Include(x => x.Custumer).ToList();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public List<OrderDto> GetOrders()
        {
            var orders = _dataContext.Orders.Include(x => x.Products).ThenInclude(x => x.Product).Include(x => x.Custumer).Include(x => x.Deliverer).ToList();
            return _mapper.Map<List<OrderDto>>(orders);
        }

        public List<Order> GetUserOrder(int userId)
        {
            List<Order> orders = null;
            var user = _dataContext.Users.Find(userId);
            if (user.UserType == UserType.Consumer)
            {
                orders = _dataContext.Orders.Where(x => x.CustumerId == userId).Include(x => x.Products).ThenInclude(x => x.Product).Include(x => x.Deliverer).ToList();
            }
            else if (user.UserType == UserType.Deliverer) 
            {
                orders = _dataContext.Orders.Where(x => x.DelivererId == userId).Include(x => x.Products).ThenInclude(x => x.Product).Include(x => x.Custumer).ToList();
            }

            if (orders == null) 
            {
                throw new Exception("There are no orders for this user!");
            }
            return orders;
        }

        private bool IsOrderValid(NewOrderDto newOrder) 
        {
            decimal totalPrice = 0;
            foreach (var item in newOrder.Products) 
            {
                totalPrice += (item.Quantity * item.Product.Price);
            }
            var user = _dataContext.Users.Where(x=> x.Id==newOrder.CustumerId);
            if (!(newOrder.Price == totalPrice && newOrder.Products.Count != 0) || newOrder.Address.Equals(string.Empty) || user == null)
            {
                return false;
            }
            else 
            {
                return true;
            }
        }
        public Order GetOrderById(int id)
        {
            var dbOrder = _dataContext.Orders.Where(x => x.Id == id)
                                             .Include(x => x.Products)
                                             .ThenInclude(x => x.Product)
                                             .Include(x => x.Custumer)
                                             .Include(x => x.Deliverer)
                                             .FirstOrDefault();

            return dbOrder;
        }
    }
}
