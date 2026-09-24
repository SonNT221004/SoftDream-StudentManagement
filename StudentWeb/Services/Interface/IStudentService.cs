using StudentWeb.Models;

namespace StudentWeb.Services.Interface
{
    public interface IStudentService
    {
        public Task<(List<StudentForListViewModel> Students, int TotalCount)> GetAllStudentsAsync(int page);
        public Task<StudentViewModel?> GetStudentByIdAsync(int id);
        public Task<StudentViewModel> CreateStudentAsync(AddStudentViewModel student);
        public Task<StudentViewModel?> UpdateStudentAsync(UpdateStudentViewModel student);
        public Task<bool> DeleteStudentAsync(int id);
        public Task<List<StudentForListViewModel>> ArrangeStudentsByNameAsync();
    }
}
