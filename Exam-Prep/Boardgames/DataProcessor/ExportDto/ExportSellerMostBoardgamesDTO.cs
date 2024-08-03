using Boardgames.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgames.DataProcessor.ExportDto
{
    public class ExportSellerMostBoardgamesDTO
    {
        
        public string Name { get; set; } = null!;        

       
        public string Website { get; set; } = null!;


        public virtual ExportSellerBoardgameDTO[] BoardgamesSellers { get; set; }
    }
}
