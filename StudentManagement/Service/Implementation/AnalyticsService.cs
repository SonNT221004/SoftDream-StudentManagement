using StudentManagement.DTO.Analytics;
using StudentManagement.Repository.Implementation;
using StudentManagement.Repository.Interface;
using StudentManagement.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StudentManagement.Service.Implementation
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        public AnalyticsService(IAnalyticsRepository analyticsRepository)
        {
            _analyticsRepository = analyticsRepository ?? throw new ArgumentNullException(nameof(analyticsRepository));
        }
        public async Task<List<TeacherClassCountDto>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken)
        {
            var result = await _analyticsRepository.GetClassCountPerTeacherAsync(cancellationToken);

            var teacherClassCount = new List<TeacherClassCountDto>();

            for (int i = 0; i < result.TeacherNames.Length; i++)
            {
                teacherClassCount.Add(new TeacherClassCountDto
                {
                    TeacherName = result.TeacherNames[i],
                    NumberOfClass = result.NumberOfClass[i]
                });
            }

            return teacherClassCount;
        }

        public async Task<List<LocationCountDto>> GetStudentCountByAddressAsync(CancellationToken cancellationToken)
        {
            var result = await _analyticsRepository.GetStudentCountByAddressAsync(cancellationToken);

            var studentAddressCount = new List<LocationCountDto>();

            for (int i = 0; i < result.Locations.Length; i++)
            {
                studentAddressCount.Add(new LocationCountDto
                {
                    Location = result.Locations[i],
                    NumberOfStudent = result.NumberOfStudent[i]
                });
            }

            return studentAddressCount;
        }

        public async Task<int> GetTotalNumberOfClasses(CancellationToken cancellationToken)
        {
            return await _analyticsRepository.GetTotalNumberOfClasses(cancellationToken);
        }

        public async Task<int> GetTotalNumberOfStudents(CancellationToken cancellationToken)
        {
            return await _analyticsRepository.GetTotalNumberOfStudents(cancellationToken);
        }

        public async Task<int> GetTotalNumberOfTeachers(CancellationToken cancellationToken)
        {
            return await _analyticsRepository.GetTotalNumberOfTeachers(cancellationToken);
        }
    }
}
