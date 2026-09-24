using AutoMapper;
using StudentManagement.Grpc;
using StudentWeb.Models;
using StudentWeb.Services.Interface;

namespace StudentWeb.Services.Implementation
{
    public class GrpcClassService : IClassService
    {
        private readonly ClassService.ClassServiceClient _classClient;
        private readonly IMapper _mapper;

        public GrpcClassService(
            ClassService.ClassServiceClient classClient,
            IMapper mapper)
        {
            _classClient = classClient;
            _mapper = mapper;
        }
        public async Task<List<ClassViewModel>> GetAllClassesAsync()
        {
            var classes = await _classClient.GetAllClassesAsync(new GetAllClassesRequest());
            var response = _mapper.Map<List<ClassViewModel>>(classes.Classes);
            return response;
        }
    }
}
