using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace CarDealer
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main()
        {
            var carDealerContext = new CarDealerContext();
            //carDealerContext.Database.EnsureDeleted();
           // carDealerContext.Database.EnsureCreated();
            string inputSuppliers = File.ReadAllText("../../../Datasets/suppliers.json");
            string inputParts = File.ReadAllText("../../../Datasets/parts.json");
            string inputCars = File.ReadAllText("../../../Datasets/cars.json");
            string inputCustomers = File.ReadAllText("../../../Datasets/customers.json");
            string inputSales = File.ReadAllText("../../../Datasets/sales.json");

            //var result = ImportSuppliers(carDealerContext, inputSuppliers);
            // var result = ImportParts(carDealerContext, inputParts);
            //var result = ImportCars(carDealerContext, inputCars);
            // var result = ImportCustomers(carDealerContext, inputCustomers);
           // var result = ImportSales(carDealerContext, inputSales);
            Console.WriteLine(GetSalesWithAppliedDiscount(carDealerContext));

        }
        //19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new
                {
                    car = new
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("0.00"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("0.00"),                    
                    priceWithDiscount = ((s.Car.PartsCars.Sum(pc => pc.Part.Price) -  ((s.Discount/100) * s.Car.PartsCars.Sum(pc => pc.Part.Price)))).ToString("0.00"),


                })
                .Take(10)
                .ToList();

            var result = JsonConvert.SerializeObject(sales,Formatting.Indented);
            return result;

        }
        //Get first 10 sales with information about the car, customer and price of the sale with and without discount. Export the list of sales to JSON 
        //18 
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales != null && c.Sales.Count() > 0)// if this customer have bought at least 1 car
                .Select(c => new
                {
                    fullName = c.Name,                       // his name
                    boughtCars = c.Sales.Count,              // count of cars he had bought
                    spentMoney =  c.Sales                    // from all his deals
                    .SelectMany(s => s.Car.PartsCars)        // get all cars with all of them parts
                    .Sum(pc => pc.Part.Price)                // and sum them by price

                })
                .OrderByDescending(c=>c.spentMoney)
                .ThenByDescending(c=>c.boughtCars)
                .ToList();

            var result = JsonConvert.SerializeObject(customers, Formatting.Indented);
            return result;
        }
        //Get all customers that have bought at least 1 car and get their names, bought cars count and total spent money on cars. Order the result list by total spent money descending, then by total bought cars again in descending order. 
        //17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsWithParts = context.Cars
                .Where(c => c.Make != null && c.Model != null)
                .Select(c => new
                {
                    car = new
                    {
                       Make =  c.Make,
                        Model =  c.Model,
                        TraveledDistance =   c.TraveledDistance,
                    },

                    parts = c.PartsCars
                    .Where(pc => pc.Part != null && pc.Part.Name != null)
                    .Select(p => new
                    {
                        
                        Name = p.Part.Name,
                        Price = p.Part.Price.ToString("F2")
                    })
                    .ToList()

                })
                .ToList();
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            };


            var result = JsonConvert.SerializeObject(carsWithParts,settings);
            return result;
        }
        //16
         public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(i => i.IsImporter == false)
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    PartsCount =  i.Parts.Count
                })
                .ToList();

            var result = JsonConvert.SerializeObject(suppliers,Formatting.Indented);
            return result;
        }
        
        //15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotas = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .OrderBy(t=>t.Model)
                .ThenByDescending(t=>t.TraveledDistance)
                .ToList();
            var result = "";
            if (toyotas != null)
            {
                 result = JsonConvert.SerializeObject(toyotas, Formatting.Indented);
            }
             
            return result;
        }
        //14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers                
                .Select(c => new
                {
                    c.Name,
                    c.BirthDate,
                    c.IsYoungDriver
                })
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    c.IsYoungDriver
                })
                .ToList();
            var result = JsonConvert.SerializeObject(customers,Formatting.Indented);
            return result;
        }
        //"Name": "Louann Holzworth",
    //"BirthDate": " 01/10/1960",
   // "IsYoungDriver": false
        //13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var dtoSales = JsonConvert.DeserializeObject<IEnumerable<SalesInputModel>>(inputJson);
            var sales = mapper.Map<IEnumerable<Sale>>(dtoSales);
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }
        //12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var dtoCustomers = JsonConvert.DeserializeObject<IEnumerable<CustomersInputModel>>(inputJson);
            var customers = mapper.Map<IEnumerable<Customer>>(dtoCustomers);

            context.Customers.AddRange(customers);
            context.SaveChanges();


           return $"Successfully imported {customers.Count()}.";
        }

        //11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var dtoCars = JsonConvert.DeserializeObject<IEnumerable<CarsInputModel>>(inputJson);
            var cars = new List<Car>();

            foreach(var dto in dtoCars)
            {
                var car = mapper.Map<Car>(dto);
                cars.Add(car);
                foreach (var partId in dto.PartsId.Distinct())
                {
                    car.PartsCars.Add(new PartCar { PartId = partId });
                }

               
            }
            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";

        }
        //10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var dtoParts = JsonConvert.DeserializeObject<IEnumerable<PartsInputModel>>(inputJson);

            var validSuplierIDs = context.Suppliers.Select(s=>s.Id).ToList();


            var parts = mapper.Map<IEnumerable<Part>>(dtoParts.Where(p=>validSuplierIDs.Contains(p.SupplierId) && p!=null));

            context.Parts.AddRange(parts);
            context.SaveChanges();
           

            return $"Successfully imported {parts.Count()}.";
        }

        //09
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            InitializeAutoMapper();
            var dtoSuppliers = JsonConvert.DeserializeObject < IEnumerable<SuppliersUnputModel>>(inputJson);
            var suppliers = mapper.Map<IEnumerable<Supplier>>(dtoSuppliers);

          
            context.AddRange(suppliers);
            context.SaveChanges();
          
            return $"Successfully imported {suppliers.Count()}.";
            ;
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