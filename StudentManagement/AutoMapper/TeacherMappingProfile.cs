using AutoMapper;
using StudentManagement.Grpc;
using StudentManagement.Model;

namespace StudentManagement.AutoMapper
{
    public class TeacherMappingProfile : Profile
    {
        public TeacherMappingProfile() 
        {
            CreateMap<Teacher, TeacherDto>();
        }
    }
}
