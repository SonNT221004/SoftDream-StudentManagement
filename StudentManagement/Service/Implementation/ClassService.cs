using StudentManagement.Model;
using StudentManagement.Repository.Interface;
using StudentManagement.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Service.Implementation
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;

        public ClassService(IClassRepository classRepository)
        {
            _classRepository = classRepository ?? throw new ArgumentNullException(nameof(classRepository));
        }
        public Task<List<Class>> GetAllClassesAsync()
        {
            return _classRepository.GetAllClassesAsync();
        }
    }
}
