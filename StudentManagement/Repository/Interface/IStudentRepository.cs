using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Repository.Interface
{
    public interface IStudentRepository
    {
        public Task<List<Student>> GetAllStudentsAsync(int page, int pageSize);
        public Task<Student?> GetStudentByIdAsync(int id);
        public Task<Student> AddStudentAsync(Student student, List<int> classIds);
        public Task<Student?> UpdateStudentAsync(Student student, List<int> classIds);
        public Task<bool> DeleteStudentAsync(int id);
        public Task<int> CountStudentsAsync();

    }
}
