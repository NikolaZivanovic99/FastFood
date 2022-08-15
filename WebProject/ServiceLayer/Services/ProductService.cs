using AutoMapper;
using DataLayer;
using DataLayer.Models;
using ServiceLayer.DTOs;
using ServiceLayer.Services.ServicesInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;

        public ProductService(DataContext dataContext,IMapper mapper)
        {
            _dataContext = dataContext;
            _mapper = mapper;
        }
        public ProductDto AddProduct(ProductDto product)
        {
            if (product.Name.Equals(string.Empty) || product.Ingredient.Equals(string.Empty) || product.Price <= 0) 
            {
                throw new Exception("Bad product data!");
            }
            var dbProduct = _mapper.Map<Product>(product);
            try
            {
                _dataContext.Add(dbProduct);
                _dataContext.SaveChanges();
            }
            catch
            {
                throw new Exception("Product with this name already exits");
            }
            return product;
        }

        public List<ProductDto> GetProducts()
        {
            var products = _dataContext.Products.ToList();
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
