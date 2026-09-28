using AutoMapper;
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
        private readonly IMapper _mapper;
        public AnalyticsService(IAnalyticsRepository analyticsRepository, IMapper mapper)
        {
            _analyticsRepository = analyticsRepository ?? throw new ArgumentNullException(nameof(analyticsRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<TeacherClassCountDto>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken)
        {
            var classesGroupByTeacher = await _analyticsRepository.GetClassCountPerTeacherAsync(cancellationToken);
            var result = _mapper.Map<List<TeacherClassCountDto>>(classesGroupByTeacher);
            return result;
        }

        public async Task<List<LocationCountDto>> GetStudentCountByAddressAsync(CancellationToken cancellationToken)
        {
            var studentAddressCount = await _analyticsRepository.GetStudentCountByAddressAsync(cancellationToken);

            var result = _mapper.Map<List<LocationCountDto>>(studentAddressCount);

            return result;
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
