namespace CourseManagementAPI.Dtos
{
    public class AuthResponseDto
    {
        public required string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public required string Email { get; set; }
        public required string DisplayName { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
