using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Resources;
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
            CreateMap<CreateCustomerForHandReceiptDto, CreateCustomerDto>();
            #endregion

            #region Items
            CreateMap<Item, ItemViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateItemDto, Item>();
            CreateMap<UpdateItemDto, Item>();
            CreateMap<Item, UpdateItemDto>();
            #endregion

            #region HandReceipts
            CreateMap<HandReceipt, HandReceiptViewModel>()
                .ForMember(x => x.Date, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(x => x.TotalCollectedAmount, x => x.MapFrom(x => x.HandReceiptItems.Sum(x => x.CollectedAmount)));
            CreateMap<CreateHandReceiptDto, HandReceipt>();
            #endregion

            #region HandReceiptItems
            CreateMap<HandReceiptItem, HandReceiptItemViewModel>()
                .ForMember(x => x.WarrantyExpiryDate, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(x => x.NotifyCustomerOfTheCost, x => x.MapFrom(x => x.NotifyCustomerOfTheCost ? Messages.Yes : Messages.No))
                .ForMember(x => x.Urgent, x => x.MapFrom(x => x.Urgent ? Messages.Yes : Messages.No))
                .ForMember(x => x.CollectionDate, x => x.MapFrom(x => x.CollectionDate != null ? x.CollectionDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(x => x.DeliveryDate, x => x.MapFrom(x => x.DeliveryDate != null ? x.DeliveryDate.Value.ToString("yyyy-MM-dd") : null));
            CreateMap<CreateHandReceiptItemDto, HandReceiptItem>();
            #endregion

            #region ReturnHandReceipts
            CreateMap<ReturnHandReceipt, ReturnHandReceiptViewModel>()
                .ForMember(x => x.Date, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateReturnHandReceiptDto, ReturnHandReceipt>();
            #endregion

            #region HandReceiptItems
            CreateMap<ReturnHandReceiptItem, ReturnHandReceiptItemViewModel>()
                .ForMember(x => x.WarrantyExpiryDate, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(x => x.DeliveryDate, x => x.MapFrom(x => x.DeliveryDate != null ? x.DeliveryDate.Value.ToString("yyyy-MM-dd") : null));
            CreateMap<CreateReturnHandReceiptItemDto, ReturnHandReceiptItem>();
            CreateMap<HandReceiptItem, CreateReturnHandReceiptItemDto>();
            #endregion

            #region Branches
            CreateMap<Branch, BranchViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateBranchDto, Branch>();
            CreateMap<UpdateBranchDto, Branch>();
            CreateMap<Branch, UpdateBranchDto>();
            #endregion
        }
    }
}
