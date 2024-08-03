namespace Boardgames.DataProcessor
{
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.Xml.Linq;

    public class Serializer
    {
        public static string ExportCreatorsWithTheirBoardgames(BoardgamesContext context)
        {
            throw new NotImplementedException();
        }

        public static string ExportSellersWithMostBoardgames(BoardgamesContext context, int year, double rating)
        {
            //var exportSellersWithMostBoardgames = context.Sellers
            //    .Where(s => s.BoardgamesSellers
            //    .Any(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating))
            //    .Select(s => new ExportSellerMostBoardgamesDTO
            //    {
            //        Name = s.Name,
            //        Website = s.Website,
            //        BoardgamesSellers = s.BoardgamesSellers
            //        .Where(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating)
            //        .Select(bs => new ExportSellerBoardgameDTO
            //        {
            //            Name = bs.Boardgame.Name,
            //            Rating = bs.Boardgame.Rating,
            //            CategoryType = bs.Boardgame.CategoryType.ToString(),
            //            Mechanics = bs.Boardgame.Mechanics

            //        })
            //        .OrderByDescending(bs => bs.Rating)
            //        .ThenBy(bs => bs.Name)
            //        .ToArray()
            //    })
            //    .OrderByDescending(s=>s.BoardgamesSellers.Count())
            //    .ThenBy(s=>s.Name)
            //    .Take(5)
            //    .ToArray();

            var sellers = context.Sellers
               .Where(s => s.BoardgamesSellers
                   .Any(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating))
               .Select(s => new
               {
                   s.Name,
                   s.Website,
                   Boardgames = s.BoardgamesSellers
                       .Where(bs => bs.Boardgame.YearPublished >= year && bs.Boardgame.Rating <= rating)
                       .Select(bs => new
                       {
                           bs.Boardgame.Name,
                           bs.Boardgame.Rating,                           
                           
                           bs.Boardgame.Mechanics,
                           Category = bs.Boardgame.CategoryType.ToString(),
                       })
                       .OrderByDescending(bs => bs.Rating)
                       .ThenBy(bs => bs.Name)
                       .ToList()
               })
               .OrderByDescending(s => s.Boardgames.Count)
               .ThenBy(s => s.Name)
               .Take(5)
               .ToList();


            var result = JsonConvert.SerializeObject(sellers, Formatting.Indented);
            return result;


                //Select the top 5 sellers

                //that have at least one boardgame that their year of publishing is greater or equal to the given year and their rating is smaller or equal to the given rating
                //.Select them with their boardgames who meet the same criteria(their year of publishing is greater or equals the given year and the rating is smaller or equal to the given rating).
                //For each seller, export their name, website and their boardgames.
                //For each boardgame, export their name, rating, mechanics and category type.
                //Order the boardgames by rating(descending), then by name(ascending).
                //Order the sellers by all boardgames(meeting above condition) count(descending), then by name(ascending).

        }
    }
}