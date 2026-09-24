using StudentManagement.Model;

namespace StudentManagement.Repository.Interface
{
    public interface ITeacherRepository
    {
        public Task<Teacher?> GetTeacherByIdAsync(int id);
        public Task<int> CountTeachersAsync();

    }
}
