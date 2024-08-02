using Invoices.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ExportDto
{
    [XmlType(nameof(Client))]
    public class ExportClientDTO
    {
        [XmlAttribute(nameof(InvoicesCount))]
        public int InvoicesCount { get; set; }


        [XmlElement(nameof(ClientName))]
        public string ClientName { get; set; }


        [XmlElement(nameof(VatNumber))]
        public string VatNumber { get; set; }


        [XmlArray(nameof(Invoices))]
        [XmlArrayItem(nameof(Invoice))]
        public ExportInvoiceDto[] Invoices { get; set; }
    }
 //   <Client InvoicesCount = "9" >
 //   < ClientName > SPEDOX, SRO</ClientName>
 //   <VatNumber>SK2023911087</VatNumber>
 //   <Invoices>
 //     <Invoice>
 //       <InvoiceNumber>1063259096</InvoiceNumber>
 //       <InvoiceAmount>167.22</InvoiceAmount>
 //       <DueDate>02/19/2023</DueDate>
 //       <Currency>EUR</Currency>
 //     </Invoice>
 //     <Invoice>
 //       <InvoiceNumber>1427940691</InvoiceNumber>
 //       <InvoiceAmount>913.13</InvoiceAmount>
 //       <DueDate>10/28/2022</DueDate>
 //       <Currency>EUR</Currency>
 //     </Invoice> 
 //     …
 //   </Invoices>
 //   … 
 //</Client> 

}
