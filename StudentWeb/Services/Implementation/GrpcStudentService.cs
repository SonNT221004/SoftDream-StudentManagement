using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;
using StudentWeb.Services.Interface;
using System.Net.WebSockets;

namespace StudentWeb.Services.Implementation
{
    public class GrpcStudentService : IStudentService
    {
        private readonly StudentService.StudentServiceClient _studentClient;
        private readonly IMapper _mapper;

        public GrpcStudentService(
            StudentService.StudentServiceClient studentClient,
            IMapper mapper)
        {
            _studentClient = studentClient;
            _mapper = mapper;
        }

        public async Task<List<StudentForListViewModel>> ArrangeStudentsByNameAsync()
        {
            var students = await _studentClient.GetAllStudentsAsync(new GetAllStudentsRequest());
            var response = _mapper.Map<List<StudentForListViewModel>>(students.Students).OrderBy(t => t.Name).ToList();
            return response;
        }

        public async Task<StudentViewModel> CreateStudentAsync(AddStudentViewModel student)
        {
           var addingStudent = _mapper.Map<AddStudentDto>(student);
           var addedStudent=  await _studentClient.AddStudentAsync(new AddStudentRequest { Student = addingStudent });
           var response = _mapper.Map<StudentViewModel>(addedStudent.Student);
           return response;
           
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var success = await _studentClient.DeleteStudentAsync(new DeleteStudentRequest { Id = id });
            return success.Success;
        }

        public async Task<(List<StudentForListViewModel> Students, int TotalCount)> GetAllStudentsAsync(int page)
        {
            var students = await _studentClient.GetAllStudentsAsync(new GetAllStudentsRequest{ Page = page, PageSize = PageConfig.PageSize });
            var response = (_mapper.Map<List<StudentForListViewModel>>(students.Students), students.TotalCount);
            return response;
        }

        public async Task<StudentViewModel?> GetStudentByIdAsync(int id)
        {
            var student = await _studentClient.GetStudentByIdAsync(new GetStudentByIdRequest { Id = id });
            var response = _mapper.Map<StudentViewModel>(student.Student);
            return response;
        }

        public async Task<StudentViewModel?> UpdateStudentAsync(UpdateStudentViewModel student)
        {
            var updateStudentDto = _mapper.Map<UpdateStudentDto>(student);
            var updatingStudent = await _studentClient.UpdateStudentAsync(new UpdateStudentRequest { Student = updateStudentDto });
            var updatedStudent = _mapper.Map<StudentViewModel>(updatingStudent?.Student);
            return updatedStudent;
        }
    }
}
