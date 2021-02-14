using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
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
    public class AdminController : ControllerBase
    {
        public IStoreServices StoreServices { get; }

        public AdminController(IStoreServices storeServices)
        {
            StoreServices = storeServices;
        }

#if OperationId
        [HttpGet("/orders", Name = nameof(GetOrders))]
#else
        [HttpGet("/orders")]
#endif
        public IEnumerable<Order> GetOrders() =>
            StoreServices.GetOrders();

#if OperationId
        [HttpGet("/orders/{id}", Name = nameof(GetOrder))]
#else
        [HttpGet("/orders/{id}")]
#endif
        public ActionResult<Order> GetOrder(Guid id)
        {
            var order = StoreServices.GetOrder(id);

            if (order is null)
                return NotFound();

            return order;
        }

#if OperationId
        [HttpGet("/orders/{id}/checkInventory", Name = nameof(CheckInventory))]
#else
        [HttpGet("/orders/{id}/checkInventory")]
#endif
        public IActionResult CheckInventory(Guid id)
        {
            try
            {
                if (!StoreServices.CheckOrderInventory(id))
                    return NotFound();

                return Ok();
            }
            catch
            {
                return Conflict();
            }
        }

#if OperationId
        [HttpGet("/orders/{id}/ship", Name = nameof(ShipOrder))]
#else
        [HttpGet("/orders/{id}/ship")]
#endif
        public IActionResult ShipOrder(Guid id)
        {
            if (!StoreServices.ShipOrder(id))
                return NotFound();

            return Ok();
        }

#if OperationId
        [HttpPut("/products/{id}/checkInventory", Name = nameof(UpdateProductInventory))]
#else
        [HttpPut("/products/{id}/checkInventory")]
#endif
        public IActionResult UpdateProductInventory(int id, InventoryUpdateRequest request)
        {
            try
            {
                StoreServices.UpdateProductInventory(id, request.countToAdd);
                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

#if OperationId
        [HttpPost("/products", Name = nameof(CreateProduct))]
#else
        [HttpPost("/products")]
#endif
        public IActionResult CreateProduct(CreateProductRequest request)
        {
            try
            {
                var newProduct = new Product(request.Id, request.Name, request.InventoryCount);
                StoreServices.CreateProduct(newProduct);
                return Created($"/products/{request.Id}", newProduct);
            }
            catch
            {
                return Conflict();
            }
        }
    }
}
