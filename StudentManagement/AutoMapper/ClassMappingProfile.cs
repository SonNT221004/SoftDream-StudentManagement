using AutoMapper;
using StudentManagement.Grpc;
using StudentManagement.Model;

namespace StudentManagement.AutoMapper
{
    public class ClassMappingProfile : Profile
    {
        public ClassMappingProfile() 
        {
            CreateMap<Class, ClassDto>();
        }
    }
}
