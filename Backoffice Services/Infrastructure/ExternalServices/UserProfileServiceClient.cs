using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class UserProfileServiceClient : IUserProfileServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserProfileServiceClient> _logger;

        public UserProfileServiceClient(
            HttpClient httpClient,
            ILogger<UserProfileServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<UserProfileDto> GetUserProfileAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/profile");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<UserProfileDto>()
                    ?? throw new Exception($"Failed to deserialize profile for user {userId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profile for {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserProfileDto>> GetUserProfilesAsync(IEnumerable<Guid> userIds)
        {
            try
            {
                var queryString = string.Join("&", userIds.Select(id => $"userIds={id}"));
                var response = await _httpClient.GetAsync($"/api/users/profiles?{queryString}");
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadFromJsonAsync<List<UserProfileDto>>()
                    ?? new List<UserProfileDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user profiles for {Count} users", userIds.Count());
                throw;
            }
        }

        public async Task<UserProfileDto> GetUserDetailsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user details. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch user details from AuthService");
                }

                var userDetails = await response.Content.ReadFromJsonAsync<UserProfileDto>();
                return userDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user details for ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserProfileDto>> GetPendingVerificationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/users/pending-verification");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch pending verifications. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch pending verifications from AuthService");
                }

                var pendingUsers = await response.Content.ReadFromJsonAsync<List<UserProfileDto>>();
                return pendingUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending verifications");
                throw;
            }
        }
    }
}