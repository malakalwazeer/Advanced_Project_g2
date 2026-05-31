using System.Text.Json.Serialization;

namespace ReportingCourseManagement.Dtos
{
    public class AuthResponseDto
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        // Maps the roles list array returning directly from the API endpoint
        [JsonPropertyName("roles")]
        public List<string> Roles { get; set; } = new List<string>();

        // Dynamic string calculation fallback property to match the controller validation check
        public string Role => Roles.FirstOrDefault() ?? string.Empty;

        [JsonPropertyName("isSuccess")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}