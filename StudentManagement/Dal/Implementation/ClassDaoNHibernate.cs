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
    public class ClassDaoNHibernate : IClassDAO
    {
        private readonly ISession _session;

        public ClassDaoNHibernate(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public async Task<int> CountClassesAsync(CancellationToken cancellationToken)
        {
            return await _session.Query<Class>().CountAsync(cancellationToken);
        }

        public async Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken)
        {
            var list = await _session.Query<Class>().ToListAsync(cancellationToken);
            return list;
        }

        public async Task<List<Class>> GetClassesByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            var idSet = new HashSet<int>(ids ?? Array.Empty<int>());
            if (idSet.Count == 0)
                return new List<Class>();

            var classes = await _session.Query<Class>().Where(c => idSet.Contains(c.Id)).ToListAsync(cancellationToken);
            return classes;
        }
    }
}
