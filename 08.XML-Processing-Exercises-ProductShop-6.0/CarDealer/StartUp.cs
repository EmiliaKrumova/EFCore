using AutoMapper;
using CarDealer.Data;
using CarDealer.Utilities;
using CarDealer.DTOs.Import;
using System.Xml.Serialization;
using System.Xml;
using CarDealer.Models;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();
            // string inputXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string inputXml = File.ReadAllText("../../../Datasets/parts.xml");
            string inputXml = File.ReadAllText("../../../Datasets/cars.xml");


            var result = ImportCars(context,inputXml);
            Console.WriteLine(result);

        }
        //11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            const string rootName = "Cars";
            IMapper mapper = InitializeAutoMapper();
            var carsDtos = XmlHelper.Deserializer<ImportCarDto>(inputXml,rootName);
            ICollection<Car> validCars = new HashSet<Car>();
            foreach (var carDto in carsDtos)
            {
                if(string.IsNullOrEmpty(carDto.Make) || string.IsNullOrEmpty(carDto.Model))
                {
                    continue;// check if there is no make and modell, and skip
                }
                Car car = mapper.Map<Car>(carDto);// mapping car from carDTO, but without collection of parts

                foreach(var partDto in carDto.Parts.DistinctBy(p=>p.PartId))// this ensure to unique partID
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
            ImportPartDto[] partDtos = XmlHelper.Deserializer<ImportPartDto>(inputXml,rootName);
            HashSet<Part> validParts = new HashSet<Part>();
            HashSet<int> validSupplierIds = context.Suppliers.Select(s=>s.Id).ToHashSet();
            foreach(var partDto in partDtos)
            {
                if(partDto.SupplierId == null || !validSupplierIds.Contains((int)partDto.SupplierId))
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
            ImportSupplierDto[] supplierDTOs = XmlHelper.Deserializer<ImportSupplierDto>(inputXml,rootName);
            var validSuppliers = new HashSet<Supplier>();
            foreach(var  supplierDTO in supplierDTOs)
            {
                if(string.IsNullOrEmpty(supplierDTO.Name))
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