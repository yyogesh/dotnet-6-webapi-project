using AutoMapper;
using ASPPRODUCT.Entity;
using ASPPRODUCT.Models;

namespace ASPPRODUCT.Handler;

public class AutoMapperHandler : Profile
{
    public AutoMapperHandler()
    {
        CreateMap<TblProduct, ProductEntity>()
        .ForMember(item => item.ProductName, source => source.MapFrom(x => x.Name))
        .ForMember(item => item.Price, source => source.MapFrom(x => Convert.ToDecimal(x.Amount)))
        .ForMember(item => item.Status, source => source.MapFrom(x => x.Amount > 10 ? "High" : "Low")).ReverseMap();
    }
}