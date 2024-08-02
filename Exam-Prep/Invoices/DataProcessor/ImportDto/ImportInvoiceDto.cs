using Invoices.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [Required]
        [Range(1000000000, 1500000000)]// this is validation for range of INT
        public int Number { get; set; }

        [Required]
        public string IssueDate { get; set; } = null!;// Deserialize DateTime as STRING!!!!
        [Required]
        public string DueDate { get; set; } = null!;// Deserialize DateTime as string!!!!
        [Required]
        public decimal Amount { get; set; }
        
        //⦁	Number – integer in range[1, 000, 000, 000…1, 500, 000, 000] (required)
        [Required]
        [Range(0,2)]
        public int CurrencyType { get; set; }// Deserialize ENUM as INT!!!!
        [Required]
        public int ClientId { get; set; }


        //    "Number": 1427940691,
        //"IssueDate": "2022-08-29T00:00:00",
        //"DueDate": "2022-10-28T00:00:00",
        //"Amount": 913.13,
        //"CurrencyType": 1,
        //"ClientId": 1

    }
}
