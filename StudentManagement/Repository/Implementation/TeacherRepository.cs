using StudentManagement.Dal.Interface;
using StudentManagement.Model;
using StudentManagement.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Repository.Implementation
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ITeacherDAO _teacherDao;

        public TeacherRepository(ITeacherDAO teacherDao)
        {
            _teacherDao = teacherDao ?? throw new ArgumentNullException(nameof(teacherDao));
        }

        public async Task<int> CountTeachersAsync(CancellationToken cancellationToken)
        {
            return await _teacherDao.CountTeachersAsync(cancellationToken);
        }

        public Task<Teacher?> GetTeacherByIdAsync(int id, CancellationToken cancellationToken)
        {
            return _teacherDao.GetTeacherById(id, cancellationToken);
        }

    }
}
