using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        public Producer()
        {
            Albums = new HashSet<Album>();//maybe LIST???

        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Name { get; set; }
        [MaxLength(60)]
        public string? Pseudonym { get; set; }
        [MaxLength (20)]
        public string? PhoneNumber { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }
    //⦁	Id – integer, Primary Key
//⦁	Name – text with max length 30 (required)
//⦁	Pseudonym – text
//⦁	PhoneNumber – text
//⦁	Albums – a collection of type Album
}
