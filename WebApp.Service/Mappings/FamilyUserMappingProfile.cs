using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class FamilyUserMappingProfile : AutoMapper.Profile
{
	public FamilyUserMappingProfile()
	{
		CreateMap<FamilyUser, FamilyUserDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
			.ForMember(dest => dest.UpdatedByUserId, opt => opt.MapFrom(src => src.UpdatedByUserId))
			.ForMember(dest => dest.Family, opt => opt.MapFrom(src => src.Family))
			.ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
			.ForMember(dest => dest.IsOwner, opt => opt.MapFrom(src => src.IsOwner));
	}
}