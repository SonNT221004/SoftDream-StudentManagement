using StudentWeb.Models;

namespace StudentWeb.Services.Interface
{
    public interface IAnalyticsService
    {
        Task<List<LocationCountForAnalyticsViewModel>> GetStudentCountByAddressAsync();
        Task<List<TeacherClassForAnalyticsViewModel>> GetClassCountPerTeacherAsync();
        public Task<int> GetTotalNumberOfStudents();
        public Task<int> GetTotalNumberOfClasses();
        public Task<int> GetTotalNumberOfTeachers();
    }
}
