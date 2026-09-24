using AutoMapper;
using StudentManagement.DTO.Student;
using StudentManagement.Grpc;
using StudentManagement.Model;

namespace StudentManagement.Automapper
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            //Map from addstudent proto to AddStudentDTO
            CreateMap<AddStudentDto, AddStudentDTO>()
                .ForMember(dest => dest.ClassIds, opt => opt.MapFrom(src => src.ClassIds.ToList()));
            // Map from AddStudentDTO used to Student entity
            CreateMap<AddStudentDTO, Student>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)))
                .ForMember(dest => dest.Classes, opt => opt.Ignore());
            //Map from Student proto to UpdateStudentDTO
            CreateMap<UpdateStudentDto, UpdateStudentDTO>()
                .ForMember(dest => dest.ClassIds, opt => opt.MapFrom(src => src.ClassIds.ToList()));
            //Map from UpdateStudentDTO to Student entity
            CreateMap<UpdateStudentDTO, Student>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateTime.Parse(src.DateOfBirth)))
                .ForMember(dest => dest.Classes, opt => opt.Ignore());

            // Map from Student entity to ViewStudentListDTO
            CreateMap<Student, ViewStudentListDTO>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")));
            // Map from ViewStudentListDTO to Student proto
            CreateMap<ViewStudentListDTO, StudentDto>();

            //Map from Student entity to ViewStudentDTO
            CreateMap<Student, ViewStudentDTO>()
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToString("yyyy-MM-dd")));
            //Map from ViewStudentDTO to Student proto
            CreateMap<ViewStudentDTO, StudentDto>();
            CreateMap<ViewStudentListDTO, StudentDtoForList>();


        }
    }
}
