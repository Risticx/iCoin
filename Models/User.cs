using iCoin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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