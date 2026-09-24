using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using StudentManagement.Model;

namespace StudentManagement.Mapping
{
    public class StudentMap : ClassMapping<Student>
    {
        public StudentMap()
        {
            Table("students");
            
            Id(x => x.Id, m =>
            {
                m.Column("id");
                m.Generator(
                    Generators.Sequence,
                    g => g.Params(new { sequence = "students_id_seq" })
                );
            });

            Property(x => x.Name, m =>
            {
                m.Length(200);
                m.NotNullable(true);
                m.Column("name");
            });

            Property(x => x.DateOfBirth, m => m.Column("date_of_birth"));

            Property(x => x.Address, m =>
            {
                m.Length(500);
                m.NotNullable(false);
                m.Column("address");
            });

            Bag(x => x.Classes, cm =>
            {
                cm.Table("class_students");
                cm.Key(k => k.Column("student_id"));
                cm.Cascade(Cascade.None);
                cm.Inverse(false); // Student is the owner of the association
            }, r => r.ManyToMany(m => m.Column("class_id")));
        }
    }
}
