using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class CategoryMappingProfile : AutoMapper.Profile
{
	public CategoryMappingProfile()
	{
		CreateMap<Category, CategoryHeaderDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.IsDefault, opt => opt.MapFrom(src => src.IsDefault))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.Icon, opt => opt.MapFrom(src => src.Icon))
			.ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color));
	}
}