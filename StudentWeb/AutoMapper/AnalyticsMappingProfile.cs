using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;

namespace StudentWeb.AutoMapper
{
    public class AnalyticsMappingProfile : Profile
    {
        public AnalyticsMappingProfile()
        {
            CreateMap<StudentAddressDto, LocationCountForAnalyticsViewModel>().ReverseMap();
            CreateMap<ClassTeacherDto, TeacherClassForAnalyticsViewModel>().ReverseMap();
        }
    }
}
