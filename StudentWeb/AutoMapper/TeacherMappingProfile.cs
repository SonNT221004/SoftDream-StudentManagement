using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;

namespace StudentWeb.AutoMapper
{
    public class TeacherMappingProfile : Profile
    {
        public TeacherMappingProfile()
        {
            CreateMap<TeacherDto, TeacherViewModel>().ReverseMap();
        }
    }
}
