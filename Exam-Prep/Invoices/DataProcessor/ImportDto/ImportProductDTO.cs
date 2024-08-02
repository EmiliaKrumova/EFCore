using Invoices.Data.Models.Enums;
using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    using static Data.Constraints;
    public class ImportProductDTO
    {
        [Required]
        [MaxLength(ProductNameMaxLenght)]
        [MinLength(ProductNameMinLenght)]
        public string Name { get; set; } = null!;
        [Required]
        //⦁	Price – in range[5.00…1000.00] (required)
        
        //RANGE attribute CAN`T convert decimal directly, and must cast STRING to DECIMAL!!!

        [Range(typeof(decimal),ProductPriceMinValue,ProductPriceMaxValue)]
        public decimal Price { get; set; }
        [Required]
        [Range(0,4)]// Validate ENUM as INT
        public int CategoryType { get; set; }

        public int[] Clients { get; set; } = null!;

        //TODO

        //⦁	Name –  length[9…30] (required)
        
    }
}
