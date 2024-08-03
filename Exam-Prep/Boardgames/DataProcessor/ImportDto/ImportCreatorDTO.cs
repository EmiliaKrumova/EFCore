using Boardgames.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Boardgames.DataProcessor.ImportDto
{
    [XmlType(nameof(Creator))]
    public class ImportCreatorDTO
    {
        /* TODO
⦁	FirstName – text with length [2, 7] (required) 
⦁	LastName – text with length [2, 7] (required)
⦁	
*/
        [XmlElement(nameof(FirstName))]
        [Required]
        [MaxLength(7)]
        [MinLength(2)]
        public string FirstName { get; set; } = null!;


        [XmlElement(nameof(LastName))]
        [Required]
        [MaxLength(7)]
        [MinLength(2)]
        public string LastName { get; set; } = null!;



        [XmlArray(nameof(Boardgames))]
        [XmlArrayItem(nameof(Boardgame))]
        public  ImportCreatorBoardGameDto[] Boardgames { get; set; }
        
    }
}
