using Boardgames.Data.Models.Enums;
using Boardgames.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType(nameof(Boardgame))]
    public class ImportCreatorBoardGameDto
    {
        //TODO
        //⦁	Name – text with length[10…20] (required)
        //⦁	Rating – double in range[1…10.00] (required)
        //⦁	YearPublished – integer in range[2018…2023] (required)

        [XmlElement(nameof(Name))]
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        
        public string Name { get; set; } = null!;



        [XmlElement(nameof(Rating))]
        [Required]
        [Range(1.00, 10.00)]
        public double Rating { get; set; }


        [XmlElement(nameof(YearPublished))]
        [Required]
        [Range(2018, 2023)]
        public int YearPublished { get; set; }


        [XmlElement(nameof(CategoryType))]
        [Required]
        [Range(0,4)]
        public int CategoryType { get; set; }



        [XmlElement(nameof(Mechanics))]
        [Required]
        public string Mechanics { get; set; } = null!;

      
    }
}
