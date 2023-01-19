using System.ComponentModel.DataAnnotations;

namespace iCoin.Models
{
    public class CoinHistory
    {
        [Required]
        public string? Id { get; set; }

        [Required]
        public List<string> PriceAndDateTime { get; set; }

        public CoinHistory() 
        {
            PriceAndDateTime = new List<string>();
        }
    }
}
