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
        public async Task<(string[] TeacherNames, int[] NumberOfClass)> GetClassCountPerTeacherAsync(CancellationToken cancellationToken)
        {
            var classes = await _classRepository.GetAllClassesAsync(cancellationToken);
            var classesGroupByTeacher = classes
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Teacher?.Name) ? "N/A" : s.Teacher.Name)
            .Select(g => new { TeacherNames = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToArray();

            var teacherNames = classesGroupByTeacher.Select(x => x.TeacherNames).ToArray();
            var NumberOfClass = classesGroupByTeacher.Select(x => x.Count).ToArray();

            return (teacherNames, NumberOfClass);
        }

        public async Task<(string[] Locations, int[] NumberOfStudent)> GetStudentCountByAddressAsync(CancellationToken cancellationToken)
        {
            var students = await _studentRepository.GetAllStudentsAsync(0,0, cancellationToken);
            var studentsGroupByAddresses = students
            .GroupBy(s => string.IsNullOrWhiteSpace(s.Address) ? "N/A" : s.Address)
            .Select(g => new { Address = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(5)
            .ToArray();

            var locations = studentsGroupByAddresses.Select(x => x.Address).ToArray();
            var numberOfStudent = studentsGroupByAddresses.Select(x => x.Count).ToArray();
            return (locations, numberOfStudent);

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
