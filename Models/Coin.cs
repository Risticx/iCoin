using System.ComponentModel.DataAnnotations;

namespace iCoin.Models
{
    public class Coin
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public string? Name { get; set; }
        
        [Required]
        public string? Symbol { get; set; }
        
        [Required]
        public decimal Price { get; set; }

        [Required]
        public string? Image { get; set; }

        public Coin() {}
    }
}
