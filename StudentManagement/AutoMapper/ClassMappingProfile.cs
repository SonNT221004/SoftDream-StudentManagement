using AutoMapper;
using StudentManagement.DTO.Student;
using StudentManagement.Grpc;
using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
