using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.ViewModels;
using Maintenance.Data.DbEntities;

namespace Maintenance.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            #region Users
            CreateMap<User, UserViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<User, UpdateUserDto>();
            #endregion

            #region Colors
            CreateMap<Color, ColorViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateColorDto, Color>();
            CreateMap<UpdateColorDto, Color>();
            CreateMap<Color, UpdateColorDto>();
            #endregion

            #region Comapnies
            CreateMap<Company, CompanyViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateCompanyDto, Company>();
            CreateMap<UpdateCompanyDto, Company>();
            CreateMap<Company, UpdateCompanyDto>();
            #endregion

            #region Customers
            CreateMap<Customer, CustomerViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();
            CreateMap<Customer, UpdateCustomerDto>();
            #endregion

            #region Items
            CreateMap<Item, ItemViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateItemDto, Item>();
            CreateMap<UpdateItemDto, Item>();
            CreateMap<Item, UpdateItemDto>();
            #endregion
        }
    }
}
