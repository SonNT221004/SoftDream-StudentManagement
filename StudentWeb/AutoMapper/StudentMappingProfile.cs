using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;

namespace StudentWeb.AutoMapper
{
    public class StudentMappingProfile : Profile
    {
        public StudentMappingProfile()
        {
            CreateMap<StudentDtoForList, StudentForListViewModel>()
             .ForMember(destination => destination.DateOfBirth,
                 options => options.MapFrom(source => DateOnly.Parse(source.DateOfBirth)));

            CreateMap<StudentForListViewModel, StudentDtoForList>()
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => source.DateOfBirth.ToString("yyyy-MM-dd")));

            CreateMap<StudentDto, StudentViewModel>()
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => DateOnly.Parse(source.DateOfBirth)));

            CreateMap<StudentViewModel, StudentDto>()
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => source.DateOfBirth.ToString("yyyy-MM-dd")));

            CreateMap<AddStudentDto, AddStudentViewModel>()
                .ForMember(destination => destination.ClassIds, 
                    options => options.MapFrom(source => source.ClassIds.ToArray()))
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => DateOnly.Parse(source.DateOfBirth)));


            CreateMap<AddStudentViewModel, AddStudentDto>()
                .ForMember(destination => destination.ClassIds,
                    options => options.MapFrom(source => source.ClassIds.ToList()))
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => source.DateOfBirth.ToString("yyyy-MM-dd")));

            CreateMap<UpdateStudentDto, UpdateStudentViewModel>()
                .ForMember(destination => destination.ClassIds,
                    options => options.MapFrom(source => source.ClassIds.ToArray()))
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => DateOnly.Parse(source.DateOfBirth)));

            CreateMap<UpdateStudentViewModel, UpdateStudentDto>()
                .ForMember(destination => destination.ClassIds,
                    options => options.MapFrom(source => source.ClassIds.ToList()))
                .ForMember(destination => destination.DateOfBirth,
                    options => options.MapFrom(source => source.DateOfBirth.ToString("yyyy-MM-dd")));
        }
    }
}
