using System;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using StudentManagement.Service.Interface;
using StudentManagement.Model;
using AutoMapper;

namespace StudentManagement.Grpc
{
    public class StudentGrpcService : StudentService.StudentServiceBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentGrpcService(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<GetAllStudentsResponse> GetAllStudents(GetAllStudentsRequest request, ServerCallContext context)
        {
            await Task.Delay(
        TimeSpan.FromSeconds(8));
            var students = await _studentService.GetAllStudentsAsync(request.Page, request.PageSize, context.CancellationToken);
            var response = new GetAllStudentsResponse
            {
                TotalCount = students.TotalCount
            };
            foreach (var s in students.Students)
            {
                response.Students.Add(_mapper.Map<StudentDtoForList>(s));
            }
            return response;
        }

        public override async Task<GetStudentByIdResponse> GetStudentById(GetStudentByIdRequest request, ServerCallContext context)
        {
            var student = await _studentService.GetStudentByIdAsync(request.Id, context.CancellationToken);
            StudentDto response = _mapper.Map<StudentDto>(student);
            return new GetStudentByIdResponse { Student = response == null ? null : response };
        }

        public override async Task<AddStudentResponse> AddStudent(AddStudentRequest request, ServerCallContext context)
        {
            var addStudentDto = _mapper.Map<DTO.Student.AddStudentDTO>(request.Student);
            var createdStudent = await _studentService.CreateStudentAsync(addStudentDto, context.CancellationToken);
            StudentDto response = _mapper.Map<StudentDto>(createdStudent);
            return new AddStudentResponse { Student = response };
        }

        public override async Task<UpdateStudentResponse> UpdateStudent(UpdateStudentRequest request, ServerCallContext context)
        {
            var student = _mapper.Map<DTO.Student.UpdateStudentDTO>(request.Student);
            var updatedStudent = await _studentService.UpdateStudentAsync(student , context.CancellationToken);
            StudentDto response = _mapper.Map<StudentDto>(updatedStudent);
            return new UpdateStudentResponse { Student = response == null ? null : response };
        }

        public override async Task<DeleteStudentResponse> DeleteStudent(DeleteStudentRequest request, ServerCallContext context)
        {
            var success = await _studentService.DeleteStudentAsync(request.Id , context.CancellationToken);
            return new DeleteStudentResponse { Success = success };
        }

        public override async Task<GetAllStudentsResponse> ArrangeStudentsByName(GetAllStudentsRequest request, ServerCallContext context)
        {
            var students = await _studentService.ArrangeStudentsByNameAsync(context.CancellationToken);
            var response = new GetAllStudentsResponse();
            foreach (var s in students)
            {
                StudentDtoForList studentDto = _mapper.Map<StudentDtoForList>(s);
                response.Students.Add(studentDto);
            }
            return response;
        }
    }
}
