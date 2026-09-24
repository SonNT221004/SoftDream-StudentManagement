using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;

namespace StudentWeb.AutoMapper
{
    public class ClassMappingProfile : Profile
    {
        public ClassMappingProfile() 
        {
            CreateMap<ClassDto, ClassViewModel>().ReverseMap();
        }
    }
}
