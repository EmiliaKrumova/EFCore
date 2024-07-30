using AutoMapper;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<SuppliersUnputModel, Supplier>();
            this.CreateMap<PartsInputModel, Part>();
            this.CreateMap<CarsInputModel, Car>();
            this.CreateMap<CustomersInputModel,Customer>();
            this.CreateMap<SalesInputModel,Sale>();
        }
    }
}
