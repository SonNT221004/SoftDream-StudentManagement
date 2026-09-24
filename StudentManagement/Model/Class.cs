using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagement.Model
{
    public class Class
    {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; } = string.Empty;
        public virtual Teacher? Teacher { get; set; }
        public virtual IList<Student> Students { get; set; } = new List<Student>();

    }
}
