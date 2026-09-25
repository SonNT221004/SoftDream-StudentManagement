using StudentManagement.Dal.Interface;
using StudentManagement.Model;
using StudentManagement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StudentManagement.Repository.Implementation
{
    public class ClassRepository : IClassRepository
    {
        private readonly IClassDAO _classDao;
        private readonly ITeacherRepository _teacherRepository;

        public ClassRepository(IClassDAO classDao, ITeacherRepository teacherRepository)
        {
            _classDao = classDao ?? throw new ArgumentNullException(nameof(classDao));
            _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        }

        public async Task<int> CountClassesAsync(CancellationToken cancellationToken)
        {
            return await _classDao.CountClassesAsync(cancellationToken);
        }

        public async Task<List<Class>> GetAllClassesAsync(CancellationToken cancellationToken)
        {
            var classesTask = await _classDao.GetAllClassesAsync(cancellationToken);
            foreach (var cls in classesTask)
            {
                if (cls.Teacher != null && cls.Teacher.Id > 0)
                {
                    var teacher = await _teacherRepository.GetTeacherByIdAsync(cls.Teacher.Id, cancellationToken);
                    cls.Teacher = teacher;
                }
            }
            return classesTask;
        }

        public Task<List<Class>> GetClassesByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken)
        {
            return _classDao.GetClassesByIdsAsync(ids, cancellationToken);
        }
    }
}
