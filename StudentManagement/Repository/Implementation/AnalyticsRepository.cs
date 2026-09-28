using StudentManagement.Dal.Interface;
using StudentManagement.Model;
using StudentManagement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Repository.Implementation
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;

        public AnalyticsRepository(IStudentRepository studentRepository, IClassRepository classRepository, ITeacherRepository teacherRepository)
        {
            _studentRepository = studentRepository ?? throw new ArgumentNullException(nameof(studentRepository));
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }
        public async Task<List<(string TeacherName, int Count)>> GetClassCountPerTeacherAsync(CancellationToken cancellationToken)
        {
            var classes = await _classRepository.GetAllClassesAsync(cancellationToken);

            var classesGroupByTeacher = classes
                .GroupBy(s => string.IsNullOrWhiteSpace(s.Teacher?.Name) ? "N/A" : s.Teacher.Name)
                .Select(g => (TeacherName: g.Key, Count: g.Count()))
                .OrderByDescending(x => x.Count)
                .ToList();

            return classesGroupByTeacher;
        }

        public async Task<List<(string LocationName, int Count)>> GetStudentCountByAddressAsync(CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllStudentsAsync(0,0, cancellationToken);
            var studentsGroupByAddresses = students
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Address) ? "N/A" : s.Address)
            .Select(g => (LocationName : g.Key, Count : g.Count()))
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToList();

            return studentsGroupByAddresses;

        }

        public async Task<int> GetTotalNumberOfClasses(CancellationToken cancellationToken)
        {
            return await _classRepository.CountClassesAsync(cancellationToken);
        } 

        public async Task<int> GetTotalNumberOfStudents(CancellationToken cancellationToken)
        {
            return await _studentRepository.CountStudentsAsync(cancellationToken);
        }

        public async Task<int> GetTotalNumberOfTeachers(CancellationToken cancellationToken)
        {
            return await _teacherRepository.CountTeachersAsync(cancellationToken);
        }
    }
}
