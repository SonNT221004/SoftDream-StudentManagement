using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.DTO.Student
{
    public class UpdateStudentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string DateOfBirth { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public IList<int> ClassIds { get; set; } = new List<int>();
    }
}
