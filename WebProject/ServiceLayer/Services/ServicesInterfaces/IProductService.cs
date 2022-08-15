using ServiceLayer.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services.ServicesInterfaces
{
    public interface IProductService
    {
        ProductDto AddProduct(ProductDto product);
        List<ProductDto> GetProducts();
    }
}
