using System.Net.Http.Json;
using AuthService.Application.DTO.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthService.Infrastructure.ExternalServices
{
    public interface IUserServiceClient
    {
        Task<string> GetUserRoleAsync(Guid userId);
        Task<UserDetailsDto> GetUserNamesAsync(Guid userId);
    }

    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserServiceClient> _logger;

        public UserServiceClient(HttpClient httpClient, IConfiguration configuration, ILogger<UserServiceClient> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            var userServiceUrl = configuration["ExternalServices:UserService:BaseUrl"];
            if (string.IsNullOrWhiteSpace(userServiceUrl))
            {
                throw new ArgumentNullException(nameof(userServiceUrl), "UserService Base URL is not configured.");
            }

            _httpClient.BaseAddress = new Uri(userServiceUrl);
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/role");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user role for UserId: {UserId}, Status Code: {StatusCode}", userId, response.StatusCode);
                    return "User"; 
                }

                var role = await response.Content.ReadFromJsonAsync<string>();
                return role ?? "User"; 
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception while fetching user role for UserId: {UserId}", userId);
                return "User";
            }
        }

        public async Task<UserDetailsDto> GetUserNamesAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/names");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user details for UserId: {UserId}, Status Code: {StatusCode}", userId, response.StatusCode);
                    throw new HttpRequestException($"Failed to fetch user details. Status Code: {response.StatusCode}");
                }

                var userDetails = await response.Content.ReadFromJsonAsync<UserDetailsDto>();

                return userDetails ?? throw new Exception("User details not found");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching user details for UserId: {UserId}", userId);
                throw;
            }
        }
    }
}
