using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using ContosoOnlineOrders.Abstractions.Models;
using ContosoOnlineOrders.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContosoOnlineOrders.Api.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("1.1")]
#if ProducesConsumes
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
#endif
    public class ShopController : ControllerBase
    {
        public IStoreServices StoreServices { get; }

        public ShopController(IStoreServices storeServices)
        {
            StoreServices = storeServices;
        }

#if OperationId
        [HttpPost("/orders", Name = nameof(CreateOrder))]
#else
        [HttpPost("/orders")]
#endif
        public IActionResult CreateOrder(Order order)
        {
            try
            {
                StoreServices.CreateOrder(order);
                return Created($"/orders/{order.Id}", order);
            }
            catch
            {
                return Conflict();
            }
        }

#if OperationId
        [HttpGet("/products", Name = nameof(GetProducts))]
#else
        [HttpGet("/products")]
#endif
        public IEnumerable<Product> GetProducts() =>
            StoreServices.GetProducts();

        [HttpGet("/products/page/{pageNumber}", Name = nameof(GetProductsPage))]
        [MapToApiVersion("1.1")]
        public IEnumerable<Product> GetProductsPage(int pageNumber)
        {
            const int pageSize = 5;
            var productsPage = StoreServices.GetProducts().Skip(pageNumber * pageSize).Take(pageSize);
            return productsPage;
        }

#if OperationId
        [HttpGet("/products/{id}", Name = nameof(GetProduct))]
#else
        [HttpGet("/products/{id}")]
#endif
        public ActionResult<Product> GetProduct(int id)
        {
            var product = StoreServices.GetProduct(id);

            if (product is null)
                return NotFound();

            return product;
        }
    }
}