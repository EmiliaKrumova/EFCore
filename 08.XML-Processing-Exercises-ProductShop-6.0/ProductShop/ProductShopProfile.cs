using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            this.CreateMap<UserDTO, User>();
            this.CreateMap<ProductDto, Product>();
            this.CreateMap<CategoryDTO, Category>();
            this.CreateMap<CategoryProductDTO, CategoryProduct>();



            //Export mapping
            this.CreateMap<Product, ExportProductDTO>()
                .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(p => p.Buyer.FirstName + " " + p.Buyer.LastName));

            this.CreateMap<Product, ExportProductDTOSecondary>();
            this.CreateMap<Category, ExportCategoryWithCountDTO>();
            this.CreateMap<User, ExportUserWithProductsDTO>();
           
        }
    }
}
