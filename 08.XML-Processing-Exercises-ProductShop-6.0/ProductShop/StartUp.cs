using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();
            //string inputXml = File.ReadAllText("../../../Datasets/users.xml");
            //string inputXml = File.ReadAllText("../../../Datasets/products.xml");
            // string inputXml = File.ReadAllText("../../../Datasets/categories.xml");
           // string inputXml = File.ReadAllText("../../../Datasets/categories-products.xml");
            string result = GetUsersWithProducts(context);
            Console.WriteLine(result);

        }
        //08
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            XmlHelper helper = new XmlHelper();
           


            var users = new ExportUserWithCountAndProductsIn
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                Users = context.Users.Where(x => x.ProductsSold.Count() > 0)
                .Select(u => new ExportUserWithProductsDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductDTO
                    {
                        Count = u.ProductsSold.Count,
                        SoldProducts = u.ProductsSold
                        .Select(s =>
                                       new ExportProductDTOSecondary
                                       {
                                           Name = s.Name,
                                           Price = s.Price
                                       })
                                       .OrderByDescending(p => p.Price)
                                       .ToArray()


                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .Take(10)
                .ToList()


            };
           
            var result = helper.Serialize(users, "Users");
            return result;
        }
        //07
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper xmlHelper = new XmlHelper();

            ExportCategoryWithCountDTO[] categories = context.Categories
                .Select(c => new ExportCategoryWithCountDTO
                {
                    Name = c.Name,

                    Count = c.CategoryProducts.Count(),

                    AveragePrice = c.CategoryProducts
                    .Select(cp => cp.Product.Price).Average(),

                    TotalRevenue = c.CategoryProducts
                    .Select(cp=>cp.Product.Price)
                    .Sum(),
                })
                .OrderByDescending(c=>c.Count)
                .ThenBy(c=>c.TotalRevenue)
                .ToArray();

            string result = xmlHelper.Serialize <ExportCategoryWithCountDTO[]>(categories, "Categories");
            return result;


        }
        //06
        public static string GetSoldProducts(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new XmlHelper();
            ExportUserDTO[] users = context.Users
                .Where(u=>u.ProductsSold!=null && u.ProductsSold.Count>0)
                .OrderBy(u=>u.LastName)
                .ThenBy(u=>u.FirstName)
                .Select(u=> new ExportUserDTO
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Products = u.ProductsSold.Select(ps=>new ExportProductDTOSecondary()
                    {
                        Name = ps.Name,
                        Price = ps.Price
                    })
                    .ToArray(),
                })
                .Take(5)
                .ToArray();

            string result = helper.Serialize(users, "Users");
            return result;
        }
        //05
        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = InitializeAutoMapper();
            XmlHelper helper = new XmlHelper();

            ExportProductDTO[] productsDTOs = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductDTO>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportProductDTO[]>(productsDTOs, "Products");
        }
        //04
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "CategoryProducts";
            XmlHelper helper = new XmlHelper();          
           
            CategoryProductDTO[] categoryproductsDTO = helper.Deserialize<CategoryProductDTO[]>(inputXml, rootName);        


            var categoriesproducts = new List<CategoryProduct>();
            foreach (var categoryproductDto in categoryproductsDTO)
            {
                if(categoryproductDto.ProductId== null || categoryproductDto.CategoryId == null)
                {
                    continue;
                }
                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(categoryproductDto);
                categoriesproducts.Add(categoryProduct);
            }
            context.AddRange(categoriesproducts);
            context.SaveChanges();
            return $"Successfully imported {categoriesproducts.Count}";

        }
        //03
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "Categories";
            XmlHelper xmlHelper = new XmlHelper();
            CategoryDTO[] categoriesDTOs = xmlHelper.Deserialize<CategoryDTO[]>(inputXml,rootName);
            ICollection<Category> categories = new List<Category>();
            foreach (var categoryDTO in categoriesDTOs)
            {
                if (string.IsNullOrEmpty(categoryDTO.Name))
                {
                    continue;
                }
                Category category = mapper.Map<Category>(categoryDTO);
                categories.Add(category);
            }
            context.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count}";

        }
        //02
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "Products";
            XmlHelper helper = new XmlHelper();
            ProductDto[] productDtos = helper.Deserialize<ProductDto[]>(inputXml,rootName);

            ICollection<Product> products = new List<Product>();

            foreach (ProductDto dto in productDtos)
            {
                if (string.IsNullOrEmpty(dto.Name))
                {
                    continue;
                }
                Product product = mapper.Map<Product>(dto);
                products.Add(product);
            }
            context.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count}";



        }
        //01
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            IMapper mapper = InitializeAutoMapper();
            const string rootName = "Users";
            XmlHelper xmlHelper = new XmlHelper();
            UserDTO[] userDTOs = xmlHelper.Deserialize<UserDTO[]>(inputXml, rootName);
            ICollection<User> validUsers = new HashSet<User>();

            foreach (UserDTO userDTO in userDTOs)
            {
                if (string.IsNullOrEmpty(userDTO.FirstName)||string.IsNullOrEmpty(userDTO.LastName))
                {
                    continue;
                }
                User user = mapper.Map<User>(userDTO);
                validUsers.Add(user);

            }
            context.AddRange(validUsers);
            context.SaveChanges();
            return $"Successfully imported {validUsers.Count}";
        }
        private static IMapper InitializeAutoMapper()
        {
            return new Mapper(
                new MapperConfiguration(cfg =>
            cfg.AddProfile<ProductShopProfile>()));
        }
    }
}