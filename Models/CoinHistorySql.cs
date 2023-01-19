using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CoinHistorySql
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public DateTime DateAndTime{ get; set; }

        public decimal Price { get; set; }
    }
}
