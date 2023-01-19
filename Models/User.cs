using System.ComponentModel.DataAnnotations;

namespace Models 
{
    public class User 
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set;}

        public string? SubscribedCoins { get; set; }
    }
}
