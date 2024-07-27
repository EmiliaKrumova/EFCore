using AutoMapper;
using CarDealer.Data;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main()
        {

        }
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            return "";
        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });
            mapper = config.CreateMapper();
        }
    }
}