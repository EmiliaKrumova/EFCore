using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ImportDto
{
    public class ImportSellersDTO
    {
        /*⦁TODO
⦁	Name – text with length [5…20] (required)
⦁	Address – text with length [2…30] (required)
⦁	Website – a string (required). First four characters are "www.", followed by upper and lower letters, digits or '-' and the last three characters are ".com".
⦁	BoardgamesSellers – collection of type BoardgameSeller*/
        [Required]
        [MinLength(5)]
        [MaxLength(20)]
        public string Name { get; set; } = null!;

        [Required]
        [MinLength(2)]
        [MaxLength(30)]
        public string Address { get; set; } = null!;
        [Required]
        public string Country { get; set; } = null!;
        [Required]
        //[RegularExpression(@"^www\.[A-Za-z0-9\-]+\.[a-z]{3}$")]
        [RegularExpression(@"^www\.[A-Za-z0-9\-]+\.com$")]
        public string Website { get; set; } = null!;
        public int[] Boardgames { get; set; } = null!;
        //      {
        //  "Name": "Asurion, LLC",
        //  "Address": "P.O. Box 234,  38-54",
        //  "Country": "Belgium",
        //"Website": "www.asurion-llc.com",
        //  "Boardgames": [
        //          1,
        //          85,
        //          81,
        //          80,
        //          5,
        //          9
        //  ]
        //  },
    }
}
