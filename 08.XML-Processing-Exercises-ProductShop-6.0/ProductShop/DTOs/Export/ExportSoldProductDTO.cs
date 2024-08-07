﻿using ProductShop.DTOs.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
   
    [XmlType("SoldProduct")]
    public class ExportSoldProductDTO
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        [XmlArrayItem("Product")]
        public ExportProductDTOSecondary[] SoldProducts { get; set; }
    }
}
