using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Category
    {
        [Required]
        public long Id { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Name { get; set;  }
    }
}