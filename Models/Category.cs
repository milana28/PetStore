using System;
using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Category
    {
        public Guid Guid { get; set; } = Guid.NewGuid();
        
        [Required]
        [StringLength(30, ErrorMessage = "Name length can't be more than 30.")]
        public string Name { get; set;  }
    }
}