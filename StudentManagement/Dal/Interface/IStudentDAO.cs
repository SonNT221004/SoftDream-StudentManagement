using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Dal.Interface
{
    public interface IStudentDAO
    {
        public Task<List<Student>> GetAllStudentsAsync(int page, int pageSize);
        public Task<Student?> GetStudentByIdAsync(int id);
        public Task<Student> AddStudentAsync(Student student);
        public Task<Student?> UpdateStudentAsync(Student student);
        public Task<bool> DeleteStudentAsync(int id);
        public Task<int> CountStudentsAsync();
    }
}
