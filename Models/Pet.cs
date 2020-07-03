using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Pet
    {
        [Required]
        public long Id { get; set; }
        
        public Category Category { get; set; }
        public string Name { get; set; }
        public PetStatuses Status { get; set; }
        public List<Tag> Tags { get; set; }
        public List<string> PhotoUrls { get; set; }
    }

    public enum PetStatuses
    {
        Available = 1,
        Pending = 2,
        Sold = 3
    }
}