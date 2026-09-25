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

        public async Task<List<Student>> GetAllStudentsAsync(int page, int pageSize, CancellationToken cancellationToken)
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

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<Student?> GetStudentByIdAsync(int id, CancellationToken cancellationToken)
        {
            var student = await _session.GetAsync<Student>(id, cancellationToken);
            return student;
        }

        public async Task<Student> AddStudentAsync(Student student, CancellationToken cancellationToken)
        {
            using var tx = _session.BeginTransaction();
            try
            {
                await _session.SaveAsync(student, cancellationToken);
                tx.Commit();
                return student;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<Student?> UpdateStudentAsync(Student student, CancellationToken cancellationToken)
        {
            using var tx = _session.BeginTransaction();
            try
            {
                // Use Merge to handle detached instances safely
                var merged = await _session.MergeAsync(student, cancellationToken) as Student;
                tx.Commit();
                return merged;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public async Task<bool> DeleteStudentAsync(int id, CancellationToken cancellationToken)
        {
            using var tx = _session.BeginTransaction();
            var student = await _session.GetAsync<Student>(id, cancellationToken);
            if (student == null)
            {
                tx.Rollback();
                return false;
            }

            await _session.DeleteAsync(student, cancellationToken);
            tx.Commit();
            return true;
        }

        public async Task<int> CountStudentsAsync(CancellationToken cancellationToken)
        {
            return await _session.Query<Student>().CountAsync(cancellationToken);
        }
    }
}
