namespace SkillSwap.Models
{
    public class ProfileExtended
    {
        public string? UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public IFormFile? ProfileImage { get; set; }
    }

}
