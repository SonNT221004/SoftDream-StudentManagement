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
        public Task<(string[] Locations, int[] NumberOfStudent)> GetStudentCountByAddressAsync();
        public Task<(string[] TeacherNames, int[] NumberOfClass)> GetClassCountPerTeacherAsync();
        public Task<int> GetTotalNumberOfStudents();
        public Task<int> GetTotalNumberOfClasses();
        public Task<int> GetTotalNumberOfTeachers();
    }
}
