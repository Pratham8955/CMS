namespace CMS.DTOs.SubjectDTO
{
    public class SubjectDTO
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = null!;

        public int DeptId { get; set; }

        public int SemId { get; set; }
    }
}
