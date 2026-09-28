using StudentManagement.DTO.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Repository.Interface
{
    public interface IAnalyticsRepository
    {
        public Task<List<(string LocationName, int Count)>> GetStudentCountByAddressAsync(CancellationToken cancellationToken);
        public Task<List<(string TeacherName, int Count)>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfStudents(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfClasses(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfTeachers(CancellationToken cancellationToken);
    }
}
