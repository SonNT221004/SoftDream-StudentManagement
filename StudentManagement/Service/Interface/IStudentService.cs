using StudentManagement.Grpc;

namespace StudentManagement.Service.Interface
{
    public interface IStudentService
    {
        public Task<(List<StudentDtoForList> Students, int TotalCount)> GetAllStudentsAsync(int page, int pageSize, CancellationToken cancellationToken);
        public Task<StudentDto?> GetStudentByIdAsync(int id, CancellationToken cancellationToken);
        public Task<StudentDto> CreateStudentAsync(AddStudentDto student, CancellationToken cancellationToken);
        public Task<StudentDto?> UpdateStudentAsync(UpdateStudentDto student, CancellationToken cancellationToken);
        public Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken);
        public Task<List<StudentDtoForList>> ArrangeStudentsByNameAsync(CancellationToken cancellationToken);
    }
}
    