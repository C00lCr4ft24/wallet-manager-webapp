using AutoMapper;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class InviteMappingProfile : Profile
{
	public InviteMappingProfile()
	{
		CreateMap<FamilyInvite, FamilyInviteDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Family, opt => opt.MapFrom(src => src.Family))
			.ForMember(dest => dest.InviterUser, opt => opt.MapFrom(src => src.Inviter))
			.ForMember(dest => dest.InvitedUser, opt => opt.MapFrom(src => src.Invitee))
			.ForMember(dest => dest.IsAccepted, opt => opt.MapFrom(src => src.IsAccepted))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			;
		CreateMap<WalletInvite, WalletInviteDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
			.ForMember(dest => dest.InviterUser, opt => opt.MapFrom(src => src.Inviter))
			.ForMember(dest => dest.InvitedUser, opt => opt.MapFrom(src => src.Invitee))
			.ForMember(dest => dest.IsAccepted, opt => opt.MapFrom(src => src.IsAccepted))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			;
	}
}