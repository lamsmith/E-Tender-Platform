using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Backoffice_Services.Application.DTO.UserManagement.Responses;
using Backoffice_Services.Domain.Paging;


namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class UserProfileServiceClient : IUserProfileServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<UserProfileServiceClient> _logger;

        public UserProfileServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<UserProfileServiceClient> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ExternalServices:UserService:BaseUrl"]);
            _logger = logger;
        }

        public async Task<UserDetailsResponseModel> GetUserDetailsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/details");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user details. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch user details from UserService");
                }

                var userDetails = await response.Content.ReadFromJsonAsync<UserDetailsResponseModel>();

                if (userDetails == null)
                {
                    throw new Exception("User details response was null");
                }

                return userDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user details for ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<UserDetailsResponseModel> GetUserProfileAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}/profile");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user profile. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch user profile");
                }

                var userProfile = await response.Content.ReadFromJsonAsync<UserDetailsResponseModel>();

                if (userProfile == null)
                {
                    throw new Exception("User profile response was null");
                }

                return userProfile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profile for ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserDetailsResponseModel>> GetUserProfilesAsync(IEnumerable<Guid> userIds)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/api/users/profiles", userIds);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user profiles. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch user profiles");
                }

                var userProfiles = await response.Content.ReadFromJsonAsync<List<UserDetailsResponseModel>>();

                if (userProfiles == null)
                {
                    throw new Exception("User profiles response was null");
                }

                return userProfiles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user profiles");
                throw;
            }
        }

        public async Task<List<PendingVerificationResponseModel>> GetPendingVerificationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/users/pending-verifications");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch pending verifications. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch pending verifications from UserService");
                }

                var pendingUsers = await response.Content.ReadFromJsonAsync<List<PendingVerificationResponseModel>>();

                if (pendingUsers == null)
                {
                    throw new Exception("Pending verifications response was null");
                }

                return pendingUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending verifications");
                throw;
            }
        }

        public async Task<PaginatedList<UserDetailsResponseModel>> GetUsersWithIncompleteProfilesAsync(PageRequest pageRequest)
        {
            try
            {
                var queryString = BuildQueryString(pageRequest);
                var response = await _httpClient.GetAsync($"api/user/incomplete-profiles{queryString}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch incomplete profiles. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch incomplete profiles from User Service");
                }

                return await response.Content.ReadFromJsonAsync<PaginatedList<UserDetailsResponseModel>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incomplete profiles");
                throw;
            }
        }

        private string BuildQueryString(PageRequest pageRequest)
        {
            var query = new List<string>
            {
                $"page={pageRequest.Page}",
                $"pageSize={pageRequest.PageSize}"
            };

            if (!string.IsNullOrWhiteSpace(pageRequest.SortBy))
            {
                query.Add($"sortBy={Uri.EscapeDataString(pageRequest.SortBy)}");
                query.Add($"isAscending={pageRequest.IsAscending}");
            }

            if (!string.IsNullOrWhiteSpace(pageRequest.Keyword))
            {
                query.Add($"keyword={Uri.EscapeDataString(pageRequest.Keyword)}");
            }

            return query.Any() ? "?" + string.Join("&", query) : string.Empty;
        }
    }
}
