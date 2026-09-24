using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;
using StudentWeb.Services.Interface;

namespace StudentWeb.Services.Implementation
{
    public class GrpcAnalyticsService : IAnalyticsService
    {
        private readonly AnalyticsService.AnalyticsServiceClient _analyticsClient;
        private readonly IMapper _mapper;

        public GrpcAnalyticsService(
            AnalyticsService.AnalyticsServiceClient analyticsClient,
            IMapper mapper)
        {
            _analyticsClient = analyticsClient;
            _mapper = mapper;
        }
        public async Task<List<TeacherClassForAnalyticsViewModel>> GetClassCountPerTeacherAsync()
        {
            var response = await _analyticsClient.GetClassCountPerTeacherAsync(new GetClassCountPerTeacherRequest());
            List<TeacherClassForAnalyticsViewModel> listData = new List<TeacherClassForAnalyticsViewModel>();
            foreach (var tc in response.ClassTeacher)
            {
                TeacherClassForAnalyticsViewModel t = _mapper.Map<TeacherClassForAnalyticsViewModel>(tc);
                listData.Add(t);
            }

            return listData;
        }

        public async Task<List<LocationCountForAnalyticsViewModel>> GetStudentCountByAddressAsync()
        {
            var response = await _analyticsClient.GetStudentCountByAddressAsync(new GetStudentCountByAddressRequest());
            List<LocationCountForAnalyticsViewModel> listData = new List<LocationCountForAnalyticsViewModel>();
            foreach (var sa in response.StudentAddress)
            {
                LocationCountForAnalyticsViewModel t = _mapper.Map<LocationCountForAnalyticsViewModel>(sa);
                listData.Add(t);
            }

            return listData;
        }

        public async Task<int> GetTotalNumberOfClasses()
        {
            var response = await _analyticsClient.GetTotalNumberOfClassesAsync(new GetTotalNumberOfClassesRequest());
            return response.NumberOfClass;
        }

        public async Task<int> GetTotalNumberOfStudents()
        {
            var response = await _analyticsClient.GetTotalNumberOfStudentsAsync(new GetTotalNumberOfStudentsRequest());
            return response.NumberOfStudent;
        }

        public async Task<int> GetTotalNumberOfTeachers()
        {
            var response = await _analyticsClient.GetTotalNumberOfTeachersAsync(new GetTotalNumberOfTeachersRequest());
            return response.NumberOfTeacher;
        }
    }
}
