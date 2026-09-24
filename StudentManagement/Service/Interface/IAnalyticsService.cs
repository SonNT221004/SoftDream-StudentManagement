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
        Task<List<LocationCountDto>> GetStudentCountByAddressAsync();
        Task<List<TeacherClassCountDto>> GetClassCountPerTeacherAsync();
        public Task<int> GetTotalNumberOfStudents();
        public Task<int> GetTotalNumberOfClasses();
        public Task<int> GetTotalNumberOfTeachers();


    }
}
