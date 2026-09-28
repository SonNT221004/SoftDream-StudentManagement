using AutoMapper;
using StudentManagement.DTO.Analytics;
namespace StudentManagement.AutoMapper
{
    public class AnalyticsMappingProfile : Profile
    {
        public AnalyticsMappingProfile()
        {
            CreateMap<(string TeacherName, int Count), TeacherClassCountDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.TeacherName))
                .ForMember(dest => dest.NumberOfClass, opt => opt.MapFrom(src => src.Count));

            CreateMap<(string LocationName, int Count), LocationCountDto>()
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.LocationName))
                .ForMember(dest => dest.NumberOfStudent, opt => opt.MapFrom(src => src.Count));
        }
    }
}