
using AutoMapper;
using WebApp.Dal.Entities;
using WebApp.Dto;

namespace WebApp.Service.Mappings;

public class LimitMappingProfile : Profile
{
	public LimitMappingProfile()
	{
		CreateMap<Limit, LimitDto>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
			.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
			.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
			.ForMember(dest => dest.CreatedByUserId, opt => opt.MapFrom(src => src.CreatedByUserId))
			.ForMember(dest => dest.MaxAmount, opt => opt.MapFrom(src => src.MaxAmount))
			.ForMember(dest => dest.CurrentAmount, opt => opt.MapFrom(src => src.CurrentAmount))
			.ForMember(dest => dest.IncludeIncome, opt => opt.MapFrom(src => src.IncludeIncome))
			.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
			.ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
			.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
			.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
			.ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted));
	}
}