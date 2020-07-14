using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PetStore.Models
{
    public class Pet
    {
        public Guid Guid { get; set; } 
        public Category Category { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
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