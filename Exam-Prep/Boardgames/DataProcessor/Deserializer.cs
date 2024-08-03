namespace Boardgames.DataProcessor
{
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Boardgames.Data;
    using Boardgames.Data.Models;
    using Boardgames.Data.Models.Enums;
    using Boardgames.Data.Utilities;
    using Boardgames.DataProcessor.ImportDto;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCreator
            = "Successfully imported creator – {0} {1} with {2} boardgames.";

        private const string SuccessfullyImportedSeller
            = "Successfully imported seller - {0} with {1} boardgames.";

        public static string ImportCreators(BoardgamesContext context, string xmlString)
        {
            XmlHelper helper = new XmlHelper();
            StringBuilder sb = new StringBuilder();
            const string xmlRoot = "Creators";
            ImportCreatorDTO[] creatorDTOs = helper.Deserialize<ImportCreatorDTO[]>(xmlString, xmlRoot);
            ICollection<Creator> validCreatorsToImport = new List<Creator>();

            foreach(var creatorDTO in creatorDTOs)
            {
                if (!IsValid(creatorDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }
                if(string.IsNullOrEmpty(creatorDTO.FirstName)|| string.IsNullOrEmpty(creatorDTO.LastName))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

               
                ICollection<Boardgame> validBoardgames = new List<Boardgame>();

                foreach(var boardgameDTO in creatorDTO.Boardgames)
                {
                    if (!IsValid(boardgameDTO))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }
                    if (string.IsNullOrEmpty(boardgameDTO.Name))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }
                    Boardgame boardgame = new Boardgame
                    {
                        Name = boardgameDTO.Name,
                        Rating = boardgameDTO.Rating,
                        YearPublished = boardgameDTO.YearPublished,
                        CategoryType = (CategoryType)boardgameDTO.CategoryType,
                        Mechanics = boardgameDTO.Mechanics,
                       
                    };
                    validBoardgames.Add(boardgame);
                }
                Creator creator = new Creator()
                {
                    FirstName = creatorDTO.FirstName,
                    LastName = creatorDTO.LastName,
                    Boardgames = validBoardgames

                };
                validCreatorsToImport.Add(creator);
                sb.AppendLine(string.Format(SuccessfullyImportedCreator, creatorDTO.FirstName, creatorDTO.LastName,validBoardgames.Count));

            }
           context.AddRange(validCreatorsToImport);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportSellers(BoardgamesContext context, string jsonString)
        {

            StringBuilder sb = new StringBuilder();
            ICollection<Seller> sellersToImport = new List<Seller>();

            var sellerDtos = JsonConvert.DeserializeObject<ImportSellersDTO[]>(jsonString);

            var validBoardgames = context.Boardgames
                .Select(b => b.Id)
                .ToList();

            foreach (var sellerDto in sellerDtos)
            {
                if (!IsValid(sellerDto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Seller newSeller = new Seller
                {
                    Name = sellerDto.Name,
                    Address = sellerDto.Address,
                    Country = sellerDto.Country,
                    Website = sellerDto.Website,
                };

                foreach (var id in sellerDto.Boardgames.Distinct())
                {
                    if (!validBoardgames.Contains(id))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    BoardgameSeller newBoardgameSeller = new BoardgameSeller
                    {
                        BoardgameId = id,
                        Seller = newSeller
                    };

                    newSeller.BoardgamesSellers.Add(newBoardgameSeller);
                }

                sellersToImport.Add(newSeller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller, newSeller.Name, newSeller.BoardgamesSellers.Count));
            }

            context.Sellers.AddRange(sellersToImport);
            context.SaveChanges();

            return sb.ToString().TrimEnd();










            var sbd = new StringBuilder();
            ICollection<Seller> validSellers = new HashSet<Seller>();
            ICollection<ImportSellersDTO> sellersDTOs = JsonConvert.DeserializeObject<ImportSellersDTO[]>(jsonString);

            foreach(var dto in sellersDTOs)
            {
                if (!IsValid(dto))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;

                }
                Seller seller = new Seller()
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Country = dto.Country,
                    Website = dto.Website,
                };

                ICollection<BoardgameSeller> validBoardgameSellers = new HashSet<BoardgameSeller>();
                foreach(var validId in dto.Boardgames.Distinct())
                {
                    if(!context.Boardgames.Any(x => x.Id == validId))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;

                    }

                    BoardgameSeller boardgameSeller = new BoardgameSeller()
                    {
                        BoardgameId = validId,
                        Seller = seller,
                    };
                    validBoardgameSellers.Add(boardgameSeller);
                }
                seller.BoardgamesSellers = validBoardgameSellers;
                validSellers.Add(seller);
                sb.AppendLine(string.Format(SuccessfullyImportedSeller,seller.Name,seller.BoardgamesSellers.Count));

            }
            context.AddRange(validSellers);
            context.SaveChanges();
            var result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
