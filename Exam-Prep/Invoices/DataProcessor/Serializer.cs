namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.DataProcessor.ExportDto;
    using Microsoft.Data.SqlClient.Server;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Diagnostics;
    using System.Runtime.Intrinsics.X86;
    using System.Xml.Linq;
    using Invoices.Data.Utilities;
    using System.Globalization;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            XmlHelper helper = new XmlHelper();
            const string xmlRoot = "Clients";

            ExportClientDTO[] clientsToexport = context.Clients
                .Where(c => c.Invoices.Any(i => DateTime.Compare(i.IssueDate, date) > 0))
                .Select(c => new ExportClientDTO
                {
                    ClientName = c.Name,
                    VatNumber = c.NumberVat,
                    InvoicesCount = c.Invoices.Count,
                    Invoices = c.Invoices
                    .OrderBy(i => i.IssueDate)
                    .ThenByDescending(i => i.DueDate)
                    .Select(i => new ExportInvoiceDto
                    {
                        InvoiceNumber = i.Number,
                        InvoiceAmount = i.Amount,
                        Currency = i.CurrencyType.ToString(),
                        DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture)
                    })
                    .ToArray()
                })
                .OrderByDescending(cl => cl.Invoices.Length)
                .ThenBy(cl => cl.ClientName)
                .ToArray();
            var result = helper.Serialize<ExportClientDTO[]>(clientsToexport,xmlRoot);
            return result;
                
            
        //     Export all clients that have at least one issued invoices, issued after the given date.
        //     For each client, export their name, vat number and invoices count.
        //     For each invoice, export its number, amount, currency and due date.
        //     Order the invoices by issue date (ascending), then by due date (descending).
        //     Order the clients by invoices count (descending), then by name (ascending).
        //NOTE: You may need to call.ToArray() function before the selection, in order to detach entities from the database and avoid runtime errors(EF Core bug).
        //NOTE: Do not forget to use CultureInfo.InvariantCulture.Use formatting("d").

           
        }
        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {
            ExportProductDTO[] productsToExport = context.Products
                .Where(p => p.ProductsClients.Any())
                .Where(p=>p.ProductsClients.Any(pc => pc.Client.Name.Length >=nameLength))
                .Select(p=>new ExportProductDTO
                {
                    Name = p.Name,
                    Price = p.Price,    
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                    .Where(pc=>pc.Client.Name.Length>=nameLength)
                    .Select(pc=> new ExportProductClientsDto
                    {
                        Name = pc.Client.Name,
                        NumberVat = pc.Client.NumberVat
                    })
                    .OrderBy(pc=>pc.Name)
                    .ToArray()
                })
                .OrderByDescending(p=>p.Clients.Count())
                .ThenBy(p=>p.Name)
                .Take(5)
                .ToArray();

          var result  = JsonConvert.SerializeObject(productsToExport, Formatting.Indented);
            return result;

            
        }
    }
}