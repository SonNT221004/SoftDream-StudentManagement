using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using StudentManagement.Model;

namespace StudentManagement.Mapping
{
    public class TeacherMap : ClassMapping<Teacher>
    {
        public TeacherMap()
        {
            Table("teachers");

            Id(x => x.Id, m =>
            {
                m.Column("id");
                m.Generator(
                    Generators.Sequence,
                    g => g.Params(new { sequence = "teachers_id_seq" })
                );
            });

            Property(x => x.Name, m =>
            {
                m.Length(200);
                m.NotNullable(true);
                m.Column("name");
            });

            Property(x => x.DateOfBirth, m => m.Column("date_of_birth"));

            Bag(x => x.Classes, cm =>
            {
                cm.Key(k => k.Column("teacher_id"));
                cm.Inverse(true);
                cm.Cascade(Cascade.All);
            }, r => r.OneToMany());
        }
    }
}
