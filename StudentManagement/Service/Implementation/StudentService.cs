using AutoMapper;
using StudentManagement.DTO.Student;
using StudentManagement.Model;
using StudentManagement.Repository.Interface;
using StudentManagement.Service.Interface;

namespace StudentManagement.Service.Implementation
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IMapper _mapper;

        public StudentService(IStudentRepository studentRepository, IMapper mapper)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<List<ViewStudentListDTO>> ArrangeStudentsByNameAsync(CancellationToken cancellationToken)
        {
           var students = await _studentRepository.GetAllStudentsAsync(0,0, cancellationToken);
           var studentDTOs = _mapper.Map<List<ViewStudentListDTO>>(students);
           return studentDTOs.OrderBy(s => s.Name).ToList();
        }

        public async Task<ViewStudentDTO> CreateStudentAsync(AddStudentDTO student, CancellationToken cancellationToken)
        {
            var studentEntity = _mapper.Map<Student>(student);
            var addedStudent = await _studentRepository.AddStudentAsync(studentEntity, student.ClassIds.ToList(), cancellationToken);
            return _mapper.Map<ViewStudentDTO>(addedStudent);
        }

        public Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken)
        {
            return _studentRepository.DeleteStudentAsync(id, cancellationToken);
        }

        public async Task<(List<ViewStudentListDTO> Students, int TotalCount)> GetAllStudentsAsync(int page, int pageSize, CancellationToken cancellationToken)
        {
            var total = await _studentRepository.CountStudentsAsync(cancellationToken);
            var students = await _studentRepository.GetAllStudentsAsync(page, pageSize, cancellationToken);
            return (_mapper.Map<List<ViewStudentListDTO>>(students), total);
        }

        public async Task<ViewStudentDTO?> GetStudentByIdAsync(int id, CancellationToken cancellationToken)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id, cancellationToken);
            return _mapper.Map<ViewStudentDTO?>(student);
        }

        public async Task<ViewStudentDTO?> UpdateStudentAsync(UpdateStudentDTO student, CancellationToken cancellationToken)
        {
            var studentEntity = _mapper.Map<Student>(student);
            var updatedStudent = await _studentRepository.UpdateStudentAsync(studentEntity, student.ClassIds.ToList(), cancellationToken);
            return _mapper.Map<ViewStudentDTO?>(updatedStudent);
        }
    }
}
