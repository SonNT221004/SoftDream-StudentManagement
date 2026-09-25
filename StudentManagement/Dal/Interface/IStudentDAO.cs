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
        public Task<List<Student>> GetAllStudentsAsync(int page, int pageSize, CancellationToken cancellationToken);
        public Task<Student?> GetStudentByIdAsync(int id, CancellationToken cancellationToken);
        public Task<Student> AddStudentAsync(Student student, CancellationToken cancellationToken);
        public Task<Student?> UpdateStudentAsync(Student student, CancellationToken cancellationToken);
        public Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken);
        public Task<int> CountStudentsAsync(CancellationToken cancellationToken);
    }
}
