using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using StudentManagement.Dal.Interface;
using StudentManagement.Model;

namespace StudentManagement.Dal.Implementation
{
    public class TeacherDaoNHibernate : ITeacherDAO
    {
        private readonly ISession _session;

        public TeacherDaoNHibernate(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<int> CountTeachersAsync(CancellationToken cancellationToken)
        {
            return await _session.Query<Teacher>().CountAsync(cancellationToken);
        }

        public Task<Teacher?> GetTeacherById(int id, CancellationToken cancellationToken)
        {
            return _session.Query<Teacher>().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }
    }
}
