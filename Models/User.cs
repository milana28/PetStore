using System.ComponentModel.DataAnnotations;

namespace PetStore.Models
{
    public class User
    {
        public long Id { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "Username length can't be more than 30.")]
        public string Username { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "Firstname length can't be more than 30.")]
        public string Firstname { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "Lastname length can't be more than 30.")]
        public string Lastname { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "Email length can't be more than 30.")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(30, ErrorMessage = "Password length can't be more than 30.")]
        public string Password { get; set; }
        
        // [Phone]
        public string Phone { get; set; }
        
        [Required]
        public int UserStatus { get; set; }
    }
}