using AutoMapper;
using Maintenance.Core.Dtos;
using Maintenance.Core.Enums;
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
            CreateMap<AddRatingToCustomerDto, Customer>();
            CreateMap<Customer, AddRatingToCustomerDto>();
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
                .ForMember(x => x.TotalCollectedAmount, x => x.MapFrom(x => x.ReceiptItems.Sum(x => x.CollectedAmount)))
                .ForMember(x => x.IsAllDelivered, x => x.MapFrom(x => x.ReceiptItems.All(x => x.MaintenanceRequestStatus 
                == MaintenanceRequestStatus.Delivered)))
                .ForMember(x => x.ItemBarcodes, x => x.MapFrom(x => string.Join(", "
                , x.ReceiptItems.Select(x => x.ItemBarcode).ToList())));
            CreateMap<CreateHandReceiptDto, HandReceipt>();
            #endregion

            #region HandReceiptItems
            CreateMap<ReceiptItem, HandReceiptItemViewModel>()
                .ForMember(x => x.NotifyCustomerOfTheCost, x => x.MapFrom(x => x.NotifyCustomerOfTheCost ? Messages.Yes : Messages.No))
                .ForMember(x => x.Urgent, x => x.MapFrom(x => x.Urgent ? Messages.Yes : Messages.No))
                .ForMember(x => x.CollectionDate, x => x.MapFrom(x => x.CollectionDate != null ? x.CollectionDate.Value.ToString("yyyy-MM-dd") : null))
                .ForMember(x => x.DeliveryDate, x => x.MapFrom(x => x.DeliveryDate != null ? x.DeliveryDate.Value.ToString("yyyy-MM-dd") : null));
            CreateMap<CreateHandReceiptItemDto, ReceiptItem>();
            CreateMap<UpdateHandReceiptItemDto, ReceiptItem>();
            CreateMap<ReceiptItem, UpdateHandReceiptItemDto>();
            #endregion

            #region ReturnHandReceipts
            CreateMap<ReturnHandReceipt, ReturnHandReceiptViewModel>()
                .ForMember(x => x.Date, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(x => x.IsAllDelivered, x => x.MapFrom(x => x.ReceiptItems.All(x => x.MaintenanceRequestStatus
                == MaintenanceRequestStatus.Delivered)))
                .ForMember(x => x.ItemBarcodes, x => x.MapFrom(x => string.Join(", "
                , x.ReceiptItems.Select(x => x.ItemBarcode).ToList())));
            CreateMap<CreateReturnHandReceiptDto, ReturnHandReceipt>();
            #endregion

            #region ReturnHandReceiptItems
            CreateMap<ReceiptItem, ReturnHandReceiptItemViewModel>()
                .ForMember(x => x.DeliveryDate, x => x.MapFrom(x => x.DeliveryDate != null
                ? x.DeliveryDate.Value.ToString("yyyy-MM-dd")
                : null));
            CreateMap<CreateReturnHandReceiptItemDto, ReceiptItem>();
            CreateMap<ReceiptItem, CreateReturnHandReceiptItemDto>();
            CreateMap<ReceiptItem, UpdateReturnHandReceiptItemDto>();
            CreateMap<UpdateReturnHandReceiptItemDto, ReceiptItem>();
            #endregion

            #region Branches
            CreateMap<Branch, BranchViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")))
                .ForMember(x => x.BranchPhoneNumbers
                    , x => x.MapFrom(x => string.Join(", ", 
                    x.BranchPhoneNumbers.Select(x => x.PhoneNumber).ToList())));
            CreateMap<CreateBranchDto, Branch>();
            CreateMap<UpdateBranchDto, Branch>();
            CreateMap<Branch, UpdateBranchDto>();
            #endregion

            #region Maintenance
            CreateMap<ReceiptItem, ReceiptItemForMaintenanceViewModel>()
                .ForMember(x => x.Urgent, x => x.MapFrom(x => x.Urgent ? Messages.Yes : Messages.No));
            #endregion

            #region BranchPhoneNumber
            CreateMap<BranchPhoneNumber, BranchPhoneNumberViewModel>()
                .ForMember(x => x.CreatedAt, x => x.MapFrom(x => x.CreatedAt.ToString("yyyy-MM-dd")));
            CreateMap<CreateBranchPhoneNumberDto, BranchPhoneNumber>();
            CreateMap<UpdateBranchPhoneNumberDto, BranchPhoneNumber>();
            CreateMap<BranchPhoneNumber, UpdateBranchPhoneNumberDto>();
            #endregion
        }
    }
}
