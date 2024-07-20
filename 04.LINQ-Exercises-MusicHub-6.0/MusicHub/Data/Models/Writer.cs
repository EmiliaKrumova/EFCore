using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        public Writer()
        {
            Songs = new HashSet<Song>();
        }
        [Key] 
        public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }
        [MaxLength(200)]
        public string? Pseudonym { get; set; }
        public virtual ICollection<Song> Songs { get; set; }
    }
    //⦁	Id – integer, Primary Key
	//Name – text with max length 20 (required)
    //Pseudonym – text
	//Songs – a collection of type Song
}
