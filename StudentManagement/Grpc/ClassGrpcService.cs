
using AutoMapper;
using Grpc.Core;
using StudentManagement.Service.Interface;

namespace StudentManagement.Grpc
{
    public class ClassGrpcService: ClassService.ClassServiceBase
    {
        private readonly IClassService _classService;
        private readonly IMapper _mapper;
        public ClassGrpcService(IClassService classService, IMapper mapper)
        {
            _classService = classService ?? throw new ArgumentNullException(nameof(classService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public override async Task<GetAllClassesResponse> GetAllClasses(GetAllClassesRequest request, ServerCallContext context)
        {
            var classes = await _classService.GetAllClassesAsync(context.CancellationToken);
            var response = new GetAllClassesResponse();
            foreach (var c in classes)
            {
                response.Classes.Add(_mapper.Map<ClassDto>(c));
            }
            return response;
        }
    }
}
