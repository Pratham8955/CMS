namespace CMS.DTOs.StudentDTO
{
    public class StudentDTOFORIMG
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly Dob { get; set; }
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int DeptId { get; set; }
        public int CurrentSemester { get; set; }
        public int GroupId { get; set; }
        public string? StudentImg { get; set; }
    }
}
