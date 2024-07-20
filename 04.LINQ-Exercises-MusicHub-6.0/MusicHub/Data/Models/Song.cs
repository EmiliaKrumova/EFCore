using MusicHub.Data.Models.Enums;
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
    public class Song
    {
        public Song()
        {
            SongPerformers = new HashSet<SongPerformer>();


        }
        [Key] public int Id { get; set; }
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }// ??? property of type DateOnly???

        [Required]
        public Genre Genre { get; set; }

        public int? AlbumId { get; set; }
        [ForeignKey(nameof(AlbumId))]
        public Album Album { get; set; }
        [Required]
        public int WriterId { get; set; }
        [ForeignKey(nameof(WriterId))]
        public Writer Writer { get; set; }
        [Required]
        public decimal Price { get; set; }
        public virtual ICollection<SongPerformer> SongPerformers { get; set; }
    }

  
//    ///*⦁	Id – integer, Primary Key
//⦁	Name – text with max length 20 (required)
//⦁	Duration – TimeSpan(required)
//⦁	CreatedOn – date(required)
//⦁	Genre – genre enumeration with possible values: "Blues, Rap, PopMusic, Rock, Jazz" (required)
//⦁	AlbumId – integer, Foreign key
//⦁	Album – the Song's Album
//⦁	WriterId – integer, Foreign key(required)
//⦁	Writer – the Song's Writer
//⦁	Price – decimal (required)
//⦁	SongPerformers – a collection of type SongPerformer
//    //
}
