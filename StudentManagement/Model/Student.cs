using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Model
{
    public class Student
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; } = string.Empty;
        public virtual DateTime DateOfBirth { get; set; }
        public virtual string Address { get; set; } = string.Empty;
        public virtual IList<Class> Classes { get; set; } = new List<Class>();
    }
}
