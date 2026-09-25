
using Grpc.Core;
using StudentManagement.Service.Implementation;
using StudentManagement.Service.Interface;

namespace StudentManagement.Grpc
{
    public class AnalyticsGrpcService : AnalyticsService.AnalyticsServiceBase
    {
        private readonly IAnalyticsService _analyticsService;
        public AnalyticsGrpcService(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService ?? throw new ArgumentNullException(nameof(analyticsService));
        }

        public override async Task<GetClassCountPerTeacherResponse> GetClassCountPerTeacher(GetClassCountPerTeacherRequest request, ServerCallContext context)
        {
            var classTeacher = await _analyticsService.GetClassCountPerTeacherAsync(context.CancellationToken);
            var response = new GetClassCountPerTeacherResponse();
            foreach(var cl in classTeacher)
            {
                ClassTeacherDto classTeacherDto = new ClassTeacherDto
                { TeacherName = cl.TeacherName, NumberOfClass = cl.NumberOfClass };
                response.ClassTeacher.Add(classTeacherDto);
            }
            return response;

        }

        public override async Task<GetStudentCountByAddressResponse> GetStudentCountByAddress(GetStudentCountByAddressRequest request, ServerCallContext context)
        {
            var studentAddress = await _analyticsService.GetStudentCountByAddressAsync(context.CancellationToken);
            var response = new GetStudentCountByAddressResponse();
            foreach (var sa in studentAddress)
            {
                StudentAddressDto studentAddressDto = new StudentAddressDto
                { LocationName = sa.Location, NumberOfStudent = sa.NumberOfStudent };
                response.StudentAddress.Add(studentAddressDto);
            }
            return response;
        }

        public override async Task<GetTotalNumberOfClassesResponse> GetTotalNumberOfClasses(GetTotalNumberOfClassesRequest request, ServerCallContext context)
        {
            int totalNumberOfClasses = await _analyticsService.GetTotalNumberOfClasses(context.CancellationToken);
            var response = new GetTotalNumberOfClassesResponse { NumberOfClass = totalNumberOfClasses };
            return response;
        }

        public override async Task<GetTotalNumberOfStudentsResponse> GetTotalNumberOfStudents(GetTotalNumberOfStudentsRequest request, ServerCallContext context)
        {
            int totalNumberOfStudents = await _analyticsService.GetTotalNumberOfStudents(context.CancellationToken);
            var response = new GetTotalNumberOfStudentsResponse { NumberOfStudent = totalNumberOfStudents };
            return response;
        }

        public override async Task<GetTotalNumberOfTeachersResponse> GetTotalNumberOfTeachers(GetTotalNumberOfTeachersRequest request, ServerCallContext context)
        {
            int totalNumberOfTeachers = await _analyticsService.GetTotalNumberOfTeachers(context.CancellationToken);
            var response = new GetTotalNumberOfTeachersResponse { NumberOfTeacher = totalNumberOfTeachers };
            return response;
        }
    }
}
 