using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.Order> AddOrder(Models.Order order)
        {
            return _order.SetOrder(order);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<List<Models.Order>> GetOrder()
        {
            return _order.GetOrders();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.Order> GetOrderById(long id)
        {
            try
            {
                if (!(id >= 1 && id <= 10))
                {
                    return BadRequest();
                }
                var order = _order.GetOrderById(id);
                if (order == null)
                {
                    return NotFound();
                }

                return order;
            }
            catch (Exception e)
            {
                 _logger.LogError("Error", e);
                 throw;
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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