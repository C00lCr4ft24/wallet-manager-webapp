using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class WalletUserMappingProfile : AutoMapper.Profile
{
	public WalletUserMappingProfile()
	{
		CreateMap<WalletUser, WalletUserDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
			.ForMember(dest => dest.UpdatedByUserId, opt => opt.MapFrom(src => src.UpdatedByUserId))
			.ForMember(dest => dest.Wallet, opt => opt.MapFrom(src => src.Wallet))
			.ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
			.ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.IsOwner));
	}
}