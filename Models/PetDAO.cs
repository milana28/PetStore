using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class PetDAO
    {
        public Guid Guid { get; set; } 
        
        [Required]
        public string Name { get; set; }
        
        public Guid CategoryGuid { get; set; }
        public PetStatuses PetStatus { get; set; }
    }
}

