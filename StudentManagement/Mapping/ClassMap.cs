using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using StudentManagement.Model;

namespace StudentManagement.Mapping
{
    public class ClassMap : ClassMapping<Class>
    {
        public ClassMap()
        {
            Table("classes");

            Id(x => x.Id, m =>
            {
                m.Column("id");
                m.Generator(
                    Generators.Sequence,
                    g => g.Params(new { sequence = "classes_id_seq" })
                ); 
            });

            Property(x => x.Name, m =>
            {
                m.Length(200);
                m.NotNullable(true);
                m.Column("name");
            });

            ManyToOne(x => x.Teacher, m =>
            {
                m.Column("teacher_id");
                m.Cascade(Cascade.None);
                m.NotNullable(false);
            });

            // Many-to-many between Class and Student using join table 'class_students'.
            Bag(x => x.Students, cm =>
            {
                cm.Table("class_students");
                cm.Key(k => k.Column("class_id"));
                cm.Inverse(true); // StudentMap is the owner in this setup
                cm.Cascade(Cascade.None);
            }, r => r.ManyToMany(m => m.Column("student_id")));
        }
    }
}
