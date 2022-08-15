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
    public class ProductController : ControllerBase
    {
        private IProductService _productService;
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        [Route("add")]
        public IActionResult AddProduct([FromBody] ProductDto product) 
        {
            try
            {
                var productNew = _productService.AddProduct(product);
                return Created("product", productNew);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("all")]
        public IActionResult GetProducts() 
        {
            try
            {
                return Ok(_productService.GetProducts());
            }
            catch 
            {
                return BadRequest();
            }
        }
    }
}
