using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.DTOs;
using ServiceLayer.Services.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [HttpGet]
        [Route("all")]
        //[Authorize]
        public IActionResult GetOrders() 
        {
            try
            {
                return Ok(_orderService.GetOrders());
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("create")]
        [Authorize]
        public IActionResult NewOrder([FromBody] NewOrderDto newOrder) 
        {
            try
            {
                var order = _orderService.AddNew(newOrder);
                return Created("order", order);
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("user/{userId}")]
        public IActionResult GetUserOrders(int userId) 
        {
            try
            {
                return Ok(_orderService.GetUserOrder(userId));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("accept")]
        [Authorize]
        public IActionResult AcceptOrder([FromBody] AcceptOrderDto acceptOrder) 
        {
            try
            {
                return Ok(_orderService.AcceptOrder(acceptOrder));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("new")]
        [Authorize]
        public IActionResult GetNewOrder()
        {
            try
            {
                return Ok(_orderService.GetNewOrder());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("end")]
        [Authorize]
        public IActionResult EndOrder([FromBody] int orderId) 
        {
            try
            {
                return Ok(_orderService.EndOrder(orderId));
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }
    }
}
