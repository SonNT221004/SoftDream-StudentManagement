namespace StudentWeb.Models
{
    public class ClassViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public TeacherViewModel? Teacher { get; set; }
    }
}
