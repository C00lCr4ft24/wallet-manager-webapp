using AutoMapper;
using WebApp.Dal.Entities;
using WebApp.Dto;
using WebApp.Service.Commands;

namespace WebApp.Service.Mappings;

public class WalletMappingProfile : Profile
{
	public WalletMappingProfile()
	{
		CreateMap<Wallet, WalletHeaderDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
			.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
			;
		CreateMap<Wallet, WalletDataDto>()
			.IncludeBase<Wallet, WalletHeaderDto>()
			.ForMember(dest => dest.Transactions, opt => opt.MapFrom(src => src.Transactions))
			.ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.WalletUsers));
	}
}