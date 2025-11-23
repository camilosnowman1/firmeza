using AutoMapper;
using Firmeza.Api.Dtos;
using Firmeza.Api.Dtos.V2;
using Firmeza.Core.Entities;

namespace Firmeza.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings (v1)
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Product Mappings (v2)
        CreateMap<Product, ProductDtoV2>()
            .ForMember(dest => dest.TotalSales, opt => opt.Ignore()); // Set manually in controller

        // Customer Mappings (v1)
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        // Customer Mappings (v2)
        CreateMap<Customer, CustomerDtoV2>()
            .ForMember(dest => dest.TotalPurchases, opt => opt.Ignore()) // Set manually in controller
            .ForMember(dest => dest.TotalSpent, opt => opt.Ignore())
            .ForMember(dest => dest.LastPurchaseDate, opt => opt.Ignore());

        // Sale Mappings
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.Customer.FullName));
        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        CreateMap<CreateSaleDto, Sale>();
        CreateMap<CreateSaleDetailDto, SaleDetail>();
        CreateMap<UpdateSaleDto, Sale>();
        CreateMap<UpdateSaleDetailDto, SaleDetail>();
        
        // Rental Mappings
        CreateMap<Rental, RentalDto>()
            .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.Customer.FullName))
            .ForMember(dest => dest.VehicleName, opt => opt.MapFrom(src => src.Vehicle.Name));
        CreateMap<CreateRentalDto, Rental>();
        CreateMap<UpdateRentalDto, Rental>();
    }
}
