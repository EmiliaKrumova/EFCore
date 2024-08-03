using Boardgames.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ExportDto
{
    public class ExportSellerBoardgameDTO
    {
        
        public string Name { get; set; } = null!;        

        public double Rating { get; set; }       

       
        public string CategoryType { get; set; }=null!; 
       
        public string Mechanics { get; set; } = null!;
    }
}
