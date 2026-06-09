using System;
using System.Collections.Generic;
using System.Text;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class FamilyMappingProfile : AutoMapper.Profile
{
	public FamilyMappingProfile()
	{
		CreateMap<Family, FamilyHeaderDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.IsShared, opt => opt.MapFrom(src => src.IsShared))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
		CreateMap<Family, FamilyDataDto>()
			.IncludeBase<Family, FamilyHeaderDto>()
			.ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.FamilyUsers))
			.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));
	}
}