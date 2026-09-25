using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Repository.Interface
{
    public interface IClassRepository
    {
        public Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken);
        public Task<List<Class>> GetClassesByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken);
        public Task<int> CountClassesAsync(CancellationToken cancellationToken);
    }
}
