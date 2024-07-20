using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MusicHub.Data.Models
{
    public class Album
    {
        public Album()
        {
            Songs = new HashSet<Song>();
        }
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(40)]
        public string Name { get; set; }
        [Required]
        public DateTime ReleaseDate { get; set; }        

        [NotMapped]
        public decimal Price => Songs.Sum(song => song.Price);

        public int? ProducerId { get; set; }
        [ForeignKey(nameof(ProducerId))]
        public Producer Producer { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
    //Id – integer, Primary Key
   // Name – text with max length 40 (required)
    //ReleaseDate – date(required)
    //Price – calculated property(the sum of all song prices in the album)
    //ProducerId – integer, foreign key
    //Producer – the Album's Producer
    //Songs – a collection of all Songs in the Album
}
