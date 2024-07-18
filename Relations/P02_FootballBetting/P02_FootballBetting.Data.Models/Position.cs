using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace P02_FootballBetting.Data.Models
{
    public class Position
    {
        [Key]
        public int PositionId { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public virtual ICollection<Player> Players { get; set; } = new HashSet<Player>();
    }
}
