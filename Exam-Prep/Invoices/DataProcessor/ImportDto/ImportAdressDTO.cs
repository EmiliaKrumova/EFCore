﻿using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType(nameof(Address))]
    public class ImportAdressDTO
    {
        [XmlElement("StreetName")]
        [Required]
        [MinLength(10)]
        [MaxLength(20)]
        public string StreetName { get; set; } = null!;


        [XmlElement("StreetNumber")]
        [Required]
        public int StreetNumber { get; set; }


        [XmlElement("PostCode")]
        [Required]
        public string PostCode { get; set; }=null!;

        [XmlElement("City")]
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        public string City { get; set; } = null!;


        [XmlElement("Country")]
        [Required]
        [MinLength(5)]
        [MaxLength(15)]
        public string Country { get; set; } = null!;







    }
}
