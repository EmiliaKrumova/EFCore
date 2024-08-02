using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel.DataAnnotations;
using Invoices.Data.Models.Enums;

namespace Invoices.Data.Models
{
    using static Constraints;
    
    public class Product
    {
        public Product()
        {
            ProductsClients = new List<ProductClient>();
        }
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(ProductNameMaxLenght)]
        public string Name { get; set; } = null!;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public CategoryType CategoryType { get; set; }

        public virtual ICollection<ProductClient> ProductsClients { get; set; }

        //TODO

        //⦁	Name –  length[9…30] (required)
        //⦁	Price – in range[5.00…1000.00] (required)



      
    }
}
