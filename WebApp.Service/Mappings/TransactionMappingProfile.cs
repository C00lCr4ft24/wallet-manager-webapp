using AutoMapper;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class TransactionMappingProfile : Profile
{
	public TransactionMappingProfile()
	{
		CreateMap<Transaction, TransactionHeaderDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
			.ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByUser))
			.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category));
		CreateMap<Transaction, TransactionDataDto>()
			.IncludeBase<Transaction, TransactionHeaderDto>()
			.ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet));
	}
}