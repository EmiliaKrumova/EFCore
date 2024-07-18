using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class User
    {
        public int UserId { get; set; }
        [MaxLength(80)]
        [Required]
        public string Username { get; set; }
        [MaxLength(80)]
        public string Name { get; set; }
        [MaxLength(128)]
        [Required]
        public string Password { get; set; }
        public string Email { get; set; }
        public decimal Balance { get; set; }

        public virtual ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
