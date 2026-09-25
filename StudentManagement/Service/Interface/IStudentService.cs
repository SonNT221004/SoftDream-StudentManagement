using StudentManagement.DTO.Student;

namespace StudentManagement.Service.Interface
{
    public interface IStudentService
    {
        public Task<(List<ViewStudentListDTO> Students, int TotalCount)> GetAllStudentsAsync(int page, int pageSize, CancellationToken cancellationToken);
        public Task<ViewStudentDTO?> GetStudentByIdAsync(int id, CancellationToken cancellationToken);
        public Task<ViewStudentDTO> CreateStudentAsync(AddStudentDTO student, CancellationToken cancellationToken);
        public Task<ViewStudentDTO?> UpdateStudentAsync(UpdateStudentDTO student, CancellationToken cancellationToken);
        public Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken);
        public Task<List<ViewStudentListDTO>> ArrangeStudentsByNameAsync(CancellationToken cancellationToken);
    }
}
    