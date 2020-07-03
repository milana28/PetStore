using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Tag
    {
        [Required]
        public long Id { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Name { get; set; }
    }
}