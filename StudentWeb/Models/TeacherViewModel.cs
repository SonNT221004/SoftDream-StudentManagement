namespace StudentWeb.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }
    }
}
