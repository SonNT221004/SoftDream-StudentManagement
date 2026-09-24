using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Dal.Interface
{
    public interface IClassDAO
    {
        public Task<List<Class>> GetAllClassesAsync();
        public Task<List<Class>> GetClassesByIdsAsync(IEnumerable<int> ids);

        public Task<int> CountClassesAsync();
    }
}
