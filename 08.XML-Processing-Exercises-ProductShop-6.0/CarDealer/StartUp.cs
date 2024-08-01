using AutoMapper;
using CarDealer.Data;
using CarDealer.Utilities;
using CarDealer.DTOs.Import;
using System.Xml.Serialization;
using System.Xml;
using CarDealer.Models;
using System.IO;
using Castle.Core.Resource;
using System.Collections.Generic;
using AutoMapper.QueryableExtensions;
using CarDealer.DTOs.Export;
using System.Xml.Linq;
using System;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            // string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string inputXml = File.ReadAllText("../../../Datasets/parts.xml");
            //string inputXml = File.ReadAllText("../../../Datasets/cars.xml");
            //string inputXml = File.ReadAllText("../../../Datasets/customers.xml");
            // string inputXml = File.ReadAllText("../../../Datasets/sales.xml");


            var result = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(result);

        }
        //19
         public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            const string rootName = "sales";
            var sales = context.Sales
                .Select(s => new ExportSaleDto
                {
                    CarSale = new ExportCarSaleDto
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = (int)s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(x=>x.Part.Price),
                    PriceWithDiscount = ((s.Car.PartsCars.Sum(pc => pc.Part.Price) - (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (s.Discount / 100))).ToString("0.####;0.####0"))


                }).ToArray();
            return XmlHelper.Serialize<ExportSaleDto[]>(sales, rootName);
            //  < sale >
            //  < car make = "Opel" model = "Omega" traveled - distance = "109910837" />
            //  < discount > 30 </ discount >
            //  < customer - name > Zada Attwoood </ customer - name >
            //  < price > 330.97 </ price >
            //  < price - with - discount > 231.68 </ price - with - discount >
            //</ sale >

        }
        //18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //IMapper mapper = InitializeAutoMapper();

            const string rootName = "customers";

            var temp = context.Customers
                            .Where(c => c.Sales.Any())
                            .Select(c => new
                            {
                                FullName = c.Name,
                                BoughtCars = c.Sales.Count,
                                SalesInfo = c.Sales.Select(s => new
                                {
                                    Prices = c.IsYoungDriver
                                    ? s.Car.PartsCars.Sum(pc => Math.Round((double)pc.Part.Price * 0.95, 2))
                                    : s.Car.PartsCars.Sum(pc => (double)pc.Part.Price)
                                }).ToArray()
                            }).ToArray();

            var customerSalesInfo = temp
                .OrderByDescending(x =>
                    x.SalesInfo.Sum(y => y.Prices))
                .Select(a => new ExportCustomerWithSalesDto()
                {
                    FullName = a.FullName,
                    BoughtCars = a.BoughtCars,
                    SpentMoney = a.SalesInfo.Sum(b => (decimal)b.Prices)
                })
                .ToArray();
            

           


            //var cutomerdtos = context.Customers
            //  .Where(c => c.Sales.Count() > 0)
            //  .ProjectTo<ExportCustomerWithSalesDto>(mapper.ConfigurationProvider)
            //  .OrderByDescending(dto => dto.SpentMoney)                
            //  .ToArray();
            return XmlHelper.Serialize<ExportCustomerWithSalesDto[]>(customerSalesInfo, rootName);






        }
        //17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "cars";
            ExportCarWithPartsDto[] carsDtos = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExportCarWithPartsDto>(mapper.ConfigurationProvider)
                .ToArray();

            var result = XmlHelper.Serialize(carsDtos, rootName);
            return result;
            //Get all cars along with their list of parts.
            //For the car get only make, model and traveled distance 
            //and for the parts get only name and price and sort all parts by price (descending).
            //Sort all cars by traveled distance (descending) and then by the model (ascending). Select top 5 records.

        }
        //16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "suppliers";

            ExportLocalSuppliersDto[] localSuppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportLocalSuppliersDto>(mapper.ConfigurationProvider)
                //.Select(s=> new ExportLocalSuppliersDto
                //{
                //    Id = s.Id, Name = s.Name,
                //    PartsCount = s.Parts.Count,
                //})
                .ToArray();

            var result = XmlHelper.Serialize<ExportLocalSuppliersDto[]>(localSuppliers, rootName);
            return result;
            //Get all suppliers that do not import parts from abroad.Get their id, name and the number of parts they can offer to supply.

        }
        //15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "cars";

            ExportCarBmwDto[] carsBMW = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportCarBmwDto>(mapper.ConfigurationProvider)
                .ToArray();

            var result = XmlHelper.Serialize<ExportCarBmwDto[]>(carsBMW, rootName);
            return result;
            //Get all cars from make BMW and order them by model alphabetically and by traveled distance descending.

        }
        //14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            const string rootName = "cars";
            IMapper mapper = InitializeAutoMapper();
            ExportCarWithDistanceDto[] cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarWithDistanceDto>(mapper.ConfigurationProvider)
                .ToArray();

            var result = XmlHelper.Serialize<ExportCarWithDistanceDto[]>(cars, rootName);
            return result;


            //Get all cars with a distance of more than 2,000,000. Order them by make, then by model alphabetically. Take top 10 records.
            //Return the list of suppliers to XML in the format provided below.

        }
        //13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            const string rootName = "Sales";
            IMapper mapper = InitializeAutoMapper();
            var salesDtos = XmlHelper.Deserializer<ImportSalesDto>(inputXml, rootName);
            ICollection<Sale> validSales = new HashSet<Sale>();
            var existingCarsIds = context.Cars.Select(c => c.Id).ToList();

            foreach (var saleDto in salesDtos)
            {
                if (saleDto.CarId == null || saleDto.CustomerId == null)
                {
                    continue;
                }
                if (!existingCarsIds.Contains(saleDto.CarId.Value))
                {
                    continue;
                }
                Sale sale = mapper.Map<Sale>(saleDto);
                validSales.Add(sale);
            }
            context.AddRange(validSales);
            context.SaveChanges();
            return $"Successfully imported {validSales.Count}";
        }
        //12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            const string rootName = "Customers";
            IMapper mapper = InitializeAutoMapper();

            var customerDtos = XmlHelper.Deserializer<ImportCustomerDto>(inputXml, rootName);
            ICollection<Customer> validCustomers = new HashSet<Customer>();
            foreach (var customerDto in customerDtos)
            {
                if (string.IsNullOrEmpty(customerDto.Name)
                    || string.IsNullOrEmpty(customerDto.BirthDate))
                {
                    continue;
                }
                Customer customer = mapper.Map<Customer>(customerDto);
                validCustomers.Add(customer);

            }
            context.AddRange(validCustomers);
            context.SaveChanges();
            return $"Successfully imported {validCustomers.Count}";
        }
        //11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string rootName = "Cars";
            IMapper mapper = InitializeAutoMapper();
            var carsDtos = XmlHelper.Deserializer<ImportCarDto>(inputXml, rootName);
            ICollection<Car> validCars = new HashSet<Car>();
            foreach (var carDto in carsDtos)
            {
                if (string.IsNullOrEmpty(carDto.Make) || string.IsNullOrEmpty(carDto.Model))
                {
                    continue;// check if there is no make and modell, and skip
                }
                Car car = mapper.Map<Car>(carDto);// mapping car from carDTO, but without collection of parts

                foreach (var partDto in carDto.Parts.DistinctBy(p => p.PartId))// this ensure to unique partID
                {
                    if (!context.Parts.Any(p => p.Id == partDto.PartId))// if this part is not in context.Parts ->skip it
                    {
                        continue;
                    }
                    PartCar partCar = new PartCar()// create new instance of mapping entity
                    {
                        PartId = partDto.PartId,// set dto.partID to mapping entity PartID
                    };
                    car.PartsCars.Add(partCar);//add part to collection of parts in current car
                }
                validCars.Add(car);// add car to hashSet of cars

            }
            context.AddRange(validCars);
            context.SaveChanges();

            return $"Successfully imported {validCars.Count}";

        }
        //10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            const string rootName = "Parts";
            IMapper mapper = InitializeAutoMapper();
            ImportPartDto[] partDtos = XmlHelper.Deserializer<ImportPartDto>(inputXml, rootName);
            HashSet<Part> validParts = new HashSet<Part>();
            HashSet<int> validSupplierIds = context.Suppliers.Select(s => s.Id).ToHashSet();
            foreach (var partDto in partDtos)
            {
                if (partDto.SupplierId == null || !validSupplierIds.Contains((int)partDto.SupplierId))
                {
                    continue;
                }
                Part part = mapper.Map<Part>(partDto);
                validParts.Add(part);
            }
            context.AddRange(validParts);
            context.SaveChanges();
            return $"Successfully imported {validParts.Count}";


        }

        //09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            const string rootName = "Suppliers";
            IMapper mapper = InitializeAutoMapper();
            ImportSupplierDto[] supplierDTOs = XmlHelper.Deserializer<ImportSupplierDto>(inputXml, rootName);
            var validSuppliers = new HashSet<Supplier>();
            foreach (var supplierDTO in supplierDTOs)
            {
                if (string.IsNullOrEmpty(supplierDTO.Name))
                {
                    continue;
                }

                Supplier supplier = mapper.Map<Supplier>(supplierDTO);
                validSuppliers.Add(supplier);

            }
            context.AddRange(validSuppliers);
            context.SaveChanges();
            return $"Successfully imported {validSuppliers.Count}";


        }
        private static IMapper InitializeAutoMapper()
            => new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));
        public static int DeleteSuppliers(CarDealerContext context)
        {
            var Todelete = context.Suppliers.Where(s => s.Id > 31).ToList();
            context.RemoveRange(Todelete);
            context.SaveChanges();
            return Todelete.Count;

        }
    }
}