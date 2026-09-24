using StudentManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.DTO.Student
{
    public class ViewStudentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public IList<Class> Classes { get; set; } = new List<Class>();

    }

}
