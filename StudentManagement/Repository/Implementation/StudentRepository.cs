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
    public class StudentRepository : IStudentRepository
    {
        private readonly IStudentDAO _studentDao;
        private readonly IClassRepository _classRepository;
        private readonly ITeacherRepository _teacherRepository;

        public StudentRepository(IStudentDAO studentDao, IClassRepository classRepository, ITeacherRepository teacherRepository)
        {
            _studentDao = studentDao ?? throw new ArgumentNullException(nameof(studentDao));
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }

        public async Task<Student> AddStudentAsync(Student student, List<int> classIds)
        {
            var classes = await _classRepository.GetClassesByIdsAsync(classIds);
            student.Classes = classes;
            return await _studentDao.AddStudentAsync(student);
        }

        public async Task<int> CountStudentsAsync()
        {
           return await _studentDao.CountStudentsAsync();
        }

        //public Task<bool> ArrangeStudentsByNameAsync()
        //{
        //    return _studentDao.ArrangeStudentsByNameAsync();
        //}

        public Task<bool> DeleteStudentAsync(int id)
        {
            return _studentDao.DeleteStudentAsync(id);
        }

        public async Task<List<Student>> GetAllStudentsAsync(int page, int pageSize)
        {
            return await _studentDao.GetAllStudentsAsync(page, pageSize);
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            var student = await _studentDao.GetStudentByIdAsync(id);
            if (student == null)
                return null;

            // if student has class ids, resolve full class objects
            if (student.Classes != null && student.Classes.Count > 0)
            {
                var ids = student.Classes.Select(c => c.Id).ToList();
                var classes = await _classRepository.GetClassesByIdsAsync(ids);
                foreach(var cls in classes)
                {
                    if (cls.Teacher != null && cls.Teacher.Id > 0)
                    {
                        var teacher = await _teacherRepository.GetTeacherByIdAsync(cls.Teacher.Id);
                        cls.Teacher = teacher;
                    }
                }
                student.Classes = classes;
            }

            return student;
        }

        public async Task<Student?> UpdateStudentAsync(Student student, List<int> classIds)
        {
            var classes = await _classRepository.GetClassesByIdsAsync(classIds);
            student.Classes = classes;
            return await _studentDao.UpdateStudentAsync(student);
        }
    }
}
