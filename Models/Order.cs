using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Order
    {
        [Required]
        public long Id { get; set; }
        
        [Required]
        public long PetId { get; set; }
        
        public int Quantity { get; set; }
        public string ShipDate { get; set; }
        public OrderStatuses Status { get; set; }
        public bool Complete { get; set; }
    }

    public enum OrderStatuses
    {
        Placed = 1,
        Approved = 2,
        Delivered = 3
    }
}