using NHibernate;
using NHibernate.Linq;
using StudentManagement.Dal.Interface;
using StudentManagement.Model;

namespace StudentManagement.Dal.Implementation
{
    public class StudentDaoNHibernate : IStudentDAO
    {
        private readonly ISession _session;

        public StudentDaoNHibernate(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<List<Student>> GetAllStudentsAsync(int page, int pageSize)
        {
            IQueryable<Student> query = _session.Query<Student>().OrderBy(s => s.Id);

            if (pageSize > 0)
            {
                if (page < 1)
                {
                    page = 1;
                }

                var skip = (page - 1) * pageSize;
                query = query.Skip(skip).Take(pageSize);
            }

            return await query.ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            var student = await _session.GetAsync<Student>(id);
            return student;
        }

        public async Task<Student> AddStudentAsync(Student student)
        {
            using var tx = _session.BeginTransaction();
            await _session.SaveAsync(student);
            tx.Commit();
            return student;
        }

        public async Task<Student?> UpdateStudentAsync(Student student)
        {
            using var tx = _session.BeginTransaction();
            // Use Merge to handle detached instances safely
            var merged = await _session.MergeAsync(student) as Student;
            tx.Commit();
            return merged;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            using var tx = _session.BeginTransaction();
            var student = await _session.GetAsync<Student>(id);
            if (student == null)
            {
                tx.Rollback();
                return false;
            }

            await _session.DeleteAsync(student);
            tx.Commit();
            return true;
        }

        public async Task<int> CountStudentsAsync()
        {
            return await _session.Query<Student>().CountAsync();
        }
    }
}
