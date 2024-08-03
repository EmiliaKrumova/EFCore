using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.Data.Models
{
    public class Seller
    {
        /*⦁TODO
⦁	Name – text with length [5…20] (required)
⦁	Address – text with length [2…30] (required)
⦁	Website – a string (required). First four characters are "www.", followed by upper and lower letters, digits or '-' and the last three characters are ".com".
⦁	BoardgamesSellers – collection of type BoardgameSeller*/
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Address { get; set; } = null!;

        [Required]
        public string Country { get; set; } = null!;

        [Required]
        [RegularExpression(@"^www\.[A-Za-z0-9\-]+\.[a-z]{3}$")]
        public string Website { get; set; }=null!;

        
        public virtual ICollection<BoardgameSeller> BoardgamesSellers  { get; set; } = new List<BoardgameSeller>();
    }
}
