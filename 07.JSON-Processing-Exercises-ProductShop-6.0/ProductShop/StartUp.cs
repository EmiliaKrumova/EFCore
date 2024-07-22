using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Diagnostics;
using System.Xml.Linq;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main()
        {

            var productShopContext = new ProductShopContext();
           // productShopContext.Database.EnsureDeleted();
           // productShopContext.Database.EnsureCreated();


           // string inputJson = File.ReadAllText("../../../Datasets/users.json");
           // string inputProducts = File.ReadAllText("../../../Datasets/products.json");
           // string inputCategories = File.ReadAllText("../../../Datasets/categories.json");
           // string inputCategoriesProducts = File.ReadAllText("../../../Datasets/categories-products.json");


           // ImportUsers(productShopContext, inputJson);
           // ImportProducts(productShopContext, inputProducts);
           // ImportCategories(productShopContext, inputCategories);
           //ImportCategoryProducts(productShopContext, inputCategoriesProducts);
            Console.WriteLine(GetUsersWithProducts(productShopContext));


        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users              
                .Where(u => u.ProductsSold.Any(b => b.BuyerId != null))
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts =new
                    {
                        count = u.ProductsSold.Where(b => b.BuyerId != null).Count(),
                        products = u.ProductsSold
                            .Where(p => p.BuyerId != null)
                            .Select(p => new
                            {
                                 name = p.Name,
                                 price = p.Price
                            })
                    }                    

                })
                .OrderByDescending(x=>x.soldProducts.products.Count());

            var resultObject = new
            {
                usersCount = users.Count(),
                users = users

            };
            var serializeSettings = new JsonSerializerSettings
            {
               NullValueHandling =  NullValueHandling.Ignore
            };

            var result = JsonConvert.SerializeObject(resultObject,serializeSettings);
            return result;
                
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories                
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = c.CategoriesProducts
                    .Average(p=>p.Product.Price).ToString("F2"),
                    totalRevenue = c.CategoriesProducts
                    .Sum(p=>p.Product.Price).ToString("F2")
                })
                .OrderByDescending(c=>c.productsCount)
                .ToList();
            ;
            var result = JsonConvert.SerializeObject(categories,Formatting.Indented);
            return result;
        }
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u=>u.ProductsSold.Any(p=>p.BuyerId!=null ))
                .Select(u=> new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                    .Where(p=>p.BuyerId !=null )
                    .Select(sp => new
                    {
                        name = sp.Name,
                        price = sp.Price,
                        buyerFirstName = sp.Buyer.FirstName,
                        buyerLastName = sp.Buyer.LastName

                    }).ToList()
                })
                .OrderBy(u=>u.lastName)
                .ThenBy(u=>u.firstName)
                .ToList();
            var result = JsonConvert.SerializeObject(users,Formatting.Indented);
            return result;
        }

         public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p=>p.Price>=500 && p.Price<=1000)
                .Select(p => new
                {
                    name = p.Name,
                    price = p.Price,
                    seller = p.Seller.FirstName+" "+p.Seller.LastName
                })
                .OrderBy(p=>p.price)
                .ThenBy(p=>p.seller)                
                .ToList();
            var result = JsonConvert.SerializeObject(products,Formatting.Indented);
            return result;
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputCategoriesProducts)
        {
            InitializeAutomapper();

            var dtoCategoryProducts = JsonConvert
                .DeserializeObject<IEnumerable<CategoryProductInputModel>>(inputCategoriesProducts);

            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(dtoCategoryProducts);
            context.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Count()}";
        }
        public static string ImportCategories(ProductShopContext context, string inputCategories)
        {
            InitializeAutomapper();

            //var settings = new JsonSerializerSettings
            //{
            //    NullValueHandling = NullValueHandling.Ignore
            //};

           // var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputCategories, settings);
            
             var dtoCategories = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputCategories);
            //var categories = mapper.Map<IEnumerable<Category>>(dtoCategories);


            var categories = mapper.Map<IEnumerable<Category>>(dtoCategories.Where(c=>c.Name!=null));
            

            context.AddRange(categories);
            context.SaveChanges();
           
            

            return $"Successfully imported {categories.Count()}";
        }
        public static string ImportProducts(ProductShopContext context, string inputProducts)
        {
            InitializeAutomapper();
            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputProducts);

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);
            context.AddRange(products);
            ;
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";

        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutomapper();
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);
            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        private static void InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });

            mapper = config.CreateMapper();
        }
    }
}