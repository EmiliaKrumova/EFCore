﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("Users")]
    public class ExportUserWithCountAndProductsIn
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        [XmlArrayItem("User")]
        public List<ExportUserWithProductsDTO> Users { get; set; }
    }
}
