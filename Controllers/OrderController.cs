using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetStore.Domain;

namespace PetStore.Controllers
{
    [ApiController]
    [Route("store/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrder _order;
        private readonly ILogger<OrderController> _logger;

        public OrderController(IOrder order, ILogger<OrderController> logger)
        {
            _order = order;
            _logger = logger;
        }

        [HttpPost]
        public ActionResult<Models.Order> AddOrder(Models.Order order)
        {
            return _order.SetOrder(order);
        }

        [HttpGet]
        public ActionResult<List<Models.Order>> GetOrder()
        {
            return _order.GetOrders();
        }

        [HttpGet("{id}")]
        public ActionResult<Models.Order> GetById(long id)
        {
            try
            {
                var order = _order.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }
                if (id >= 1 && id < 10)
                {
                    return order;
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception e)
            {
                 _logger.LogError("Error", e);
                 throw;
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Models.Order> DeleteOrder(long id)
        {
            var order = _order.GetOrderById(id);
            if (order == null)
            {
                return NotFound();
            }
            
            return _order.DeleteOrder(order);
        }
    }
}