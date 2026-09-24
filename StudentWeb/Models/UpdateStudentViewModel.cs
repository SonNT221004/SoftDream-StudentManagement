namespace StudentWeb.Models
{
    public class UpdateStudentViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public int[] ClassIds { get; set; } = [];
    }
}
