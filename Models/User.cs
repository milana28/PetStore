using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class User
    {
        [Required]
        public long Id { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Username { get; set; }
        
        [Required]
        [StringLength(30)]
        public string FirstName { get; set; }
        
        [Required]
        [StringLength(30)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Email { get; set; }
        
        [Required]
        [StringLength(30)]
        public string Password { get; set; }
        
        public string Phone { get; set; }
        public int UserStatus { get; set; }
    }
}