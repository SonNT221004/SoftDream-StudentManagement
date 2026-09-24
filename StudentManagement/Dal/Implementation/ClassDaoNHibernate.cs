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

        public async Task<int> CountClassesAsync()
        {
            return await _session.Query<Class>().CountAsync();
        }

        public async Task<List<Class>> GetAllClassesAsync()
        {
            var list = await _session.Query<Class>().ToListAsync();
            return list;
        }

        public async Task<List<Class>> GetClassesByIdsAsync(IEnumerable<int> ids)
        {
            var idSet = new HashSet<int>(ids ?? Array.Empty<int>());
            if (idSet.Count == 0)
                return new List<Class>();

            var classes = await _session.Query<Class>().Where(c => idSet.Contains(c.Id)).ToListAsync();
            return classes;
        }
    }
}
