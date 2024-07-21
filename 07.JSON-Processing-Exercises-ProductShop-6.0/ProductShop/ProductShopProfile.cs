using AutoMapper;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {
            //mapping UserInputModel (-DataTranferObject)  to User (-Model in DB)
            this.CreateMap<UserInputModel, User>();

            this.CreateMap<ProductInputModel, Product>();
            this.CreateMap<CategoryInputModel, Category>();
            this.CreateMap<CategoryProductInputModel, CategoryProduct>();
        }
    }
}
