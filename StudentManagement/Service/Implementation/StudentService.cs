using AutoMapper;
using StudentManagement.DTO.Student;
using StudentManagement.Model;
using StudentManagement.Repository.Interface;
using StudentManagement.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<List<ViewStudentListDTO>> ArrangeStudentsByNameAsync()
        {
           var students = await _studentRepository.GetAllStudentsAsync(0,0);
           var studentDTOs = _mapper.Map<List<ViewStudentListDTO>>(students);
           return studentDTOs.OrderBy(s => s.Name).ToList();
        }

        public async Task<ViewStudentDTO> CreateStudentAsync(AddStudentDTO student)
        {
            var studentEntity = _mapper.Map<Student>(student);
            var addedStudent = await _studentRepository.AddStudentAsync(studentEntity, student.ClassIds.ToList());
            return _mapper.Map<ViewStudentDTO>(addedStudent);
        }

        public Task<bool> DeleteStudentAsync(int id)
        {
            return _studentRepository.DeleteStudentAsync(id);
        }

        public async Task<(List<ViewStudentListDTO> Students, int TotalCount)> GetAllStudentsAsync(int page, int pageSize)
        {
            var total = await _studentRepository.CountStudentsAsync();
            var students = await _studentRepository.GetAllStudentsAsync(page, pageSize);
            return (_mapper.Map<List<ViewStudentListDTO>>(students), total);
        }

        public async Task<ViewStudentDTO?> GetStudentByIdAsync(int id)
        {
            var student = await _studentRepository.GetStudentByIdAsync(id);
            return _mapper.Map<ViewStudentDTO?>(student);
        }

        public async Task<ViewStudentDTO?> UpdateStudentAsync(UpdateStudentDTO student)
        {
            var studentEntity = _mapper.Map<Student>(student);
            var updatedStudent = await _studentRepository.UpdateStudentAsync(studentEntity, student.ClassIds.ToList());
            return _mapper.Map<ViewStudentDTO?>(updatedStudent);
        }
    }
}
