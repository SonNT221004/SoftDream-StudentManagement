using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Dal.Interface
{
    public interface ITeacherDAO
    {
        public Task<Teacher?> GetTeacherById(int id);
        public Task<int> CountTeachersAsync();
    }
}
