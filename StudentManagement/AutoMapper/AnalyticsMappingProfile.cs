using AutoMapper;
using StudentManagement.Grpc;
namespace StudentManagement.AutoMapper
{
    public class AnalyticsMappingProfile : Profile
    {
        public AnalyticsMappingProfile()
        {
            CreateMap<(string TeacherName, int Count), ClassTeacherDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.TeacherName))
                .ForMember(dest => dest.NumberOfClass, opt => opt.MapFrom(src => src.Count));

            CreateMap<(string LocationName, int Count), StudentAddressDto>()
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src => src.LocationName))
                .ForMember(dest => dest.NumberOfStudent, opt => opt.MapFrom(src => src.Count));
        }
    }
}