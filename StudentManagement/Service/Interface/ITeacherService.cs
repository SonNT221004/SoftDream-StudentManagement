using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Service.Interface
{
    public interface ITeacherService
    {
        public Task<List<Teacher>> GetAllTeachersAsync();
    }
}
