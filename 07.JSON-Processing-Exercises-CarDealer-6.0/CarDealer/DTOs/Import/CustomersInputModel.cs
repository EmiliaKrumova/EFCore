using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarDealer.DTOs.Import
{
    public class CustomersInputModel
    {
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public bool IsYoungDriver { get; set; }

    }
    //"name": "Marcelle Griego",
   // "birthDate": "1990-10-04T00:00:00",
    //"isYoungDriver": true
}
