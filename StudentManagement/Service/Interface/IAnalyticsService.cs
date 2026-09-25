using StudentManagement.DTO.Analytics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Service.Interface
{
    public interface IAnalyticsService
    {
        Task<List<LocationCountDto>> GetStudentCountByAddressAsync(CancellationToken cancellationToken);
        Task<List<TeacherClassCountDto>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfStudents(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfClasses(CancellationToken cancellationToken);
        public Task<int> GetTotalNumberOfTeachers(CancellationToken cancellationToken);


    }
}
