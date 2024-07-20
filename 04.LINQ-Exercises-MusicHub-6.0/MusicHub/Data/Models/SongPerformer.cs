using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicHub.Data.Models
{
    public class SongPerformer
    {
        public int SongId { get; set; }
        [Required]
        public Song Song { get; set; }

        public int PerformerId { get; set; }
        [Required]
        public Performer Performer { get; set; }
    }
    //⦁	SongId – integer, Primary Key
///⦁	Song – the performer's Song (required)
//⦁	PerformerId – integer, Primary Key
//⦁	Performer – the Song's Performer (required)

}
