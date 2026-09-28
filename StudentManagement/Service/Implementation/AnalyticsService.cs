using AutoMapper;
using StudentManagement.Grpc;
using StudentManagement.Repository.Interface;
using StudentManagement.Service.Interface;

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
        public async Task<List<ClassTeacherDto>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken)
        {
            var classesGroupByTeacher = await _analyticsRepository.GetClassCountPerTeacherAsync(cancellationToken);
            var result = _mapper.Map<List<ClassTeacherDto>>(classesGroupByTeacher);
            return result;
        }

        public async Task<List<StudentAddressDto>> GetStudentCountByAddressAsync(CancellationToken cancellationToken)
        {
            var studentAddressCount = await _analyticsRepository.GetStudentCountByAddressAsync(cancellationToken);

            var result = _mapper.Map<List<StudentAddressDto>>(studentAddressCount);

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
