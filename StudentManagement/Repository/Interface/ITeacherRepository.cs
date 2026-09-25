using StudentManagement.Model;

namespace StudentManagement.Repository.Interface
{
    public interface ITeacherRepository
    {
        public Task<Teacher?> GetTeacherByIdAsync(int id, CancellationToken cancellationToken);
        public Task<int> CountTeachersAsync(CancellationToken cancellationToken);

    }
}
