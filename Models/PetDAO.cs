using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class PetDAO
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        public Guid CategoryGuid { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public int Status { get; set; }
        
        public Guid TagGuid { get; set; }
        public string PhotoUrl { get; set; }
    }
}