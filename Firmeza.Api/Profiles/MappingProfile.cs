using AutoMapper;
using Firmeza.Api.Dtos;
using Firmeza.Core.Entities;

namespace Firmeza.Api.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product Mappings
        CreateMap<Product, ProductDto>();
        CreateMap<CreateProductDto, Product>();
        CreateMap<UpdateProductDto, Product>();

        // Customer Mappings
        CreateMap<Customer, CustomerDto>();
        CreateMap<CreateCustomerDto, Customer>();
        CreateMap<UpdateCustomerDto, Customer>();

        // Sale Mappings
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.CustomerFullName, opt => opt.MapFrom(src => src.Customer.FullName));
        CreateMap<SaleDetail, SaleDetailDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
        CreateMap<CreateSaleDto, Sale>();
        CreateMap<CreateSaleDetailDto, SaleDetail>();
        CreateMap<UpdateSaleDto, Sale>();
        CreateMap<UpdateSaleDetailDto, SaleDetail>();
    }
}