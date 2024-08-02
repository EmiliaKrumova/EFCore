using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Client))]
    public class ImportClientDTO
    {
        [XmlElement("Name")]
        [Required]
        [MinLength(10)]
        [MaxLength(25)]
        public string Name { get; set; } = null!;
        [XmlElement("NumberVat")]
        [Required]
        [MinLength(10)]
        [MaxLength(15)]
        public string NumberVat { get; set; } = null!;
        [XmlArray("Addresses")]
        [XmlArrayItem("Address")]

        public ImportAdressDTO[] Addresses { get; set; }



    }
}
