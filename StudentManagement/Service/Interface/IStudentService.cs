using StudentManagement.DTO.Student;

namespace StudentManagement.Service.Interface
{
    public interface IStudentService
    {
        public Task<(List<ViewStudentListDTO> Students, int TotalCount)> GetAllStudentsAsync(int page, int pageSize);
        public Task<ViewStudentDTO?> GetStudentByIdAsync(int id);
        public Task<ViewStudentDTO> CreateStudentAsync(AddStudentDTO student);
        public Task<ViewStudentDTO?> UpdateStudentAsync(UpdateStudentDTO student);
        public Task<bool> DeleteStudentAsync(int id);
        public Task<List<ViewStudentListDTO>> ArrangeStudentsByNameAsync();
    }
}
    