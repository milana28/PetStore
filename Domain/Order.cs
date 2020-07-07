using System.Collections.Generic;
using PetStore.Models;

namespace PetStore.Domain
{
    public interface IOrder
    {
        Models.Order SetOrder(Models.Order oder);
        List<Models.Order> GetOrders();
        Models.Order GetOrderById(long id);
        Models.Order DeleteOrder(Models.Order order);

    }
    public class Order : IOrder
    {
        
        private readonly List<Models.Order> _orders = new List<Models.Order>()
        {
            new Models.Order()
            {
                Id = 1,
                PetGuid = "someGuid",
                Quantity = 1,
                ShipDate = "2020-07-01T13:19:11.456Z",
                Status = OrderStatuses.Approved,
                Complete = true
            },
        };

        public Models.Order SetOrder(Models.Order order)
        {
            _orders.Add(order);
            return order;
        }
        public List<Models.Order> GetOrders()
        {
            return _orders;
        }

        public Models.Order GetOrderById(long id)
        {
            return _orders.Find(o => o.Id == id);
        }

        public Models.Order DeleteOrder(Models.Order order)
        { 
            _orders.Remove(order);
            return order;
        }
    }
}