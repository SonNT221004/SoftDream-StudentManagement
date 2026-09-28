
using StudentManagement.Grpc;

namespace StudentManagement.Service.Interface
{
    public interface IAnalyticsService
    {
        Task<List<StudentAddressDto>> GetStudentCountByAddressAsync(CancellationToken cancellationToken);
        Task<List<ClassTeacherDto>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfStudents(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfClasses(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfTeachers(CancellationToken cancellationToken);


    }
}
