using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Data.SqlTypes;
using System.Globalization;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDto, Supplier>();
            this.CreateMap<ImportPartDto, Part>();
            this.CreateMap<ImportCarDto, Car>()
                .ForSourceMember(s => s.Parts, opt => opt.DoNotValidate());
            this.CreateMap<ImportCustomerDto, Customer>()
                .ForMember(c => c.BirthDate, opt => opt.MapFrom(c => DateTime.Parse(c.BirthDate, CultureInfo.InvariantCulture)));
            this.CreateMap<ImportSalesDto, Sale>()
                 .ForMember(d => d.CarId,
                    opt => opt.MapFrom(s => s.CarId.Value))
                 .ForMember(d => d.CustomerId, opt => opt.MapFrom(s => s.CustomerId.Value));
            this.CreateMap<Car, ExportCarWithDistanceDto>();


            //export


            this.CreateMap<Car, ExportCarBmwDto>();
            this.CreateMap<Supplier, ExportLocalSuppliersDto>()
                .ForMember(s => s.PartsCount, opt => opt.MapFrom(s => s.Parts.Count));

            //Mapping DTO with nested collection of Another DTO !!! 
            this.CreateMap<Part, ExportPartCarDto>();

            //Using LINQ to select and order by "nested model"
            this.CreateMap<Car, ExportCarWithPartsDto>()
                .ForMember(dto => dto.Parts, opt => opt.MapFrom(dbModel => dbModel.PartsCars
                .Select(dbModel => dbModel.Part)
                .OrderByDescending(part => part.Price)
                .ToArray()));



            



            

        }
    }
}

