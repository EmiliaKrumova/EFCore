using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;

        public static void Main()
        {

            var productShopContext = new ProductShopContext();
            productShopContext.Database.EnsureDeleted();
            productShopContext.Database.EnsureCreated();
            

            string inputJson = File.ReadAllText("../../../Datasets/users.json");
            string inputProducts = File.ReadAllText("../../../Datasets/products.json");
            string inputCategories = File.ReadAllText("../../../Datasets/categories.json");
            string inputCategoriesProducts = File.ReadAllText("../../../Datasets/categories-products.json");

            
            ImportUsers(productShopContext, inputJson);
            ImportProducts(productShopContext, inputProducts);
            ImportCategories(productShopContext, inputCategories);
           var result = ImportCategoryProducts(productShopContext, inputCategoriesProducts);
            Console.WriteLine(result);


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