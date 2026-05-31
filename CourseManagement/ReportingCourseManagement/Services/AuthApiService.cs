using System.Net.Http.Json;
using ReportingCourseManagement.Dtos;
using System.Net.Http.Json;

namespace ReportingCourseManagement.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _httpClient;

        public AuthApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            try
            {
                // Sends the request using the pre-configured BaseAddress from Program.cs
                var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

                    // Force IsSuccess to true if the API returned a valid response token successfully
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        result.IsSuccess = true;
                    }
                    return result;
                }

                string errorMessage = $"API Error (Status {(int)response.StatusCode})";
                try
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (errorResult != null && !string.IsNullOrEmpty(errorResult.Message))
                    {
                        errorMessage = errorResult.Message;
                    }
                }
                catch
                {
                    // Fallback if the error payload isn't structured as an AuthResponseDto
                    var rawContent = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(rawContent))
                    {
                        errorMessage = rawContent;
                    }
                }

                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = errorMessage
                };
            }
            catch (Exception ex)
            {
                // Catches network disconnects, invalid ports, or offline API instances
                return new AuthResponseDto
                {
                    IsSuccess = false,
                    Message = $"Connection error to API: {ex.Message}"
                };
            }
        }
    }
}