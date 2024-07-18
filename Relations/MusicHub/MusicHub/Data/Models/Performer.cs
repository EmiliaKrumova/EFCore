using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MusicHub.Data.Models
{
    public class Performer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(20)]
        public string LastName { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public decimal NetWorth { get; set;}

        public virtual ICollection<SongPerformer> PerformerSongs { get; set; }
    }
    //⦁	Id – integer, Primary Key
//⦁	FirstName – text with max length 20 (required) 
//⦁	LastName – text with max length 20 (required) 
//⦁	Age – integer(required)
//⦁	NetWorth – decimal (required)
//⦁	PerformerSongs – a collection of type SongPerformer

}
