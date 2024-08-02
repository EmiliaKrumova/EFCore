namespace Invoices.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.Data.Utilities;
    using Invoices.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            XmlHelper helper = new XmlHelper();
            const string xmlRoot = "Clients";
            ImportClientDTO[] clientsDto = helper.Deserialize<ImportClientDTO[]>(xmlString,xmlRoot);
            ICollection<Client> validClients = new List<Client>();
            StringBuilder sb = new StringBuilder();
            foreach(ImportClientDTO clientDto in clientsDto)
            {
                if (!IsValid(clientDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                ICollection<Address> validAdresses = new List<Address>();

                foreach(ImportAdressDTO addressDto in clientDto.Addresses)
                {
                    if (!IsValid(addressDto))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    Address validAdress = new Address()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country,

                    };
                    validAdresses.Add(validAdress);
                }
                Client client = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                    Addresses = validAdresses,// EF will import both ValidClients and validAdresses !!!!
                };
                validClients.Add(client);
                sb.AppendLine(string.Format(SuccessfullyImportedClients, clientDto.Name));
            }
            context.Clients.AddRange(validClients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();
            ImportInvoiceDto[] invoiceDtos = JsonConvert.DeserializeObject<ImportInvoiceDto[]>(jsonString);
            ICollection<Invoice> validInvoices = new List<Invoice>();
            var clientsInDbIds = context.Clients.Select(c=>c.Id);
            foreach(var invoiceDTO in invoiceDtos)
            {
                if (!IsValid(invoiceDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                //Validation for DateTime from STRING
                bool isIssueDateValid = DateTime.TryParse
                    (invoiceDTO.IssueDate,CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime issueDate);
                if (!isIssueDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                // this is method with 2 results -> BOOL IsDueDateValid, and real dueDate in DATETIME format!!!!!!!!

                bool isDueDateValid = DateTime.TryParse
                    (invoiceDTO.DueDate,CultureInfo.InvariantCulture, DateTimeStyles.None,out DateTime dueDate);
                // this is method with 2 results -> bool Isvalid, and real dueDate in DATETIME format
                if (!isDueDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }
                if(DateTime.Compare(dueDate,issueDate) < 0)// Compare if dueDate is before issueDate
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                if(!clientsInDbIds.Contains(invoiceDTO.ClientId))
                    // check if this client exist in db, before import invoice for this client
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice invoice = new Invoice()
                {
                    Number = invoiceDTO.Number,
                    IssueDate = issueDate, // this is result from TRYPARSE method for issueDate
                    DueDate = dueDate, // this is result from TRYPARSE method for dueDate
                    Amount = invoiceDTO.Amount,
                    CurrencyType = (CurrencyType)invoiceDTO.CurrencyType,// Cast int CurrencyType to ENUM ot type CurrencyType
                    ClientId = invoiceDTO.ClientId,
                };
                validInvoices.Add(invoice);
                sb.AppendLine(string.Format(SuccessfullyImportedInvoices, invoice.Number));

            }
            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            
            StringBuilder sb = new StringBuilder();
            

            ICollection<Product> validProducts = new List<Product>();
            ImportProductDTO[] productDTOs = JsonConvert.DeserializeObject<ImportProductDTO[]>(jsonString);



            foreach (var productDto in productDTOs)
            {
                if (!IsValid(productDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }
                Product product = new Product// create MAP from DTO to Database Model
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryType = (CategoryType)productDto.CategoryType,

                };
                // this is mapping table from Clients and Products
                ICollection<ProductClient> validProductClients = new List<ProductClient>();// this is mapping table from Clients and Products
                foreach (var clientId in productDto.Clients.Distinct()) // Get only UNIQUE ID`s from dto array of ClientID`s
                {
                    if (!context.Clients.Any(cl => cl.Id == clientId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    ProductClient productClient = new ProductClient()
                    {
                        ClientId = clientId,

                        Product = product


                    };
                    validProductClients.Add(productClient);
                }
                product.ProductsClients = validProductClients; // set directly collection of productsClients to Product DB model
                validProducts.Add(product);


                //int[] clients = validProductClients.Select(pc=>pc.ClientId).ToArray();
                //Console.WriteLine($"productID: {product.Id}, productName: {product.Name},--- clientsIDs::{String.Join(" ,",clients)}");


                sb.AppendLine(string.Format(SuccessfullyImportedProducts, productDto.Name, validProductClients.Count));
            }
             context.Products.AddRange(validProducts);
             context.SaveChanges();
             return sb.ToString().TrimEnd();
           
                
            


            
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
