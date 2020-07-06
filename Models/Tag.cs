using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class Tag
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "Name length can't be more than 30.")]
        public string Name { get; set; }
    }
}