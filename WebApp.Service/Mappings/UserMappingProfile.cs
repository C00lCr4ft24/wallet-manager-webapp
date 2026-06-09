using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class UserMappingProfile : AutoMapper.Profile
{
	public UserMappingProfile()
	{
		CreateMap<User, UserMinimalDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name));
		CreateMap<User, UserHeaderDto>()
			.IncludeBase<User, UserMinimalDto>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
			.ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
			.ForMember(dest => dest.TotalBalance, opt => opt.MapFrom((User src) => src.WalletUsers.Sum((WalletUser wu) => wu.Wallet.Transactions.Sum((Transaction t) => t.Amount))));
	}
}