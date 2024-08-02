using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Invoices.Data.Models
{
    using static Constraints;
    public class Client
    {
        
        public Client()
        {
            ProductsClients = new List<ProductClient>();
            Invoices = new List<Invoice>();
            Addresses = new List<Address>();
        }
        [Key]

        public int Id { get; set; }
        [Required]
        [MaxLength(25)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        public string NumberVat { get; set; } = null!;
        public virtual ICollection<ProductClient> ProductsClients { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }

        //TODO

        //⦁	Name –  length[10…25] (required)
        //⦁	NumberVat – text with length[10…15] (required)
        


    }
}
