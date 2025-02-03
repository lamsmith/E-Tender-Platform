using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthServiceClient> _logger;

        public AuthServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AuthServiceClient> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ExternalServices:AuthService:BaseUrl"]);
            _logger = logger;
        }

        public async Task<UserDetailsDto> GetUserDetailsAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/users/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch user details. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch user details from AuthService");
                }

                var userDetails = await response.Content.ReadFromJsonAsync<UserDetailsDto>();
                return userDetails;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user details for ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserDetailsDto>> GetPendingVerificationsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/users/pending-verification");
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch pending verifications. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to fetch pending verifications from AuthService");
                }

                var pendingUsers = await response.Content.ReadFromJsonAsync<List<UserDetailsDto>>();
                return pendingUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pending verifications");
                throw;
            }
        }

        public async Task<bool> UpdateUserVerificationStatusAsync(Guid userId, bool isApproved, string reason = null)
        {
            try
            {
                var request = new
                {
                    IsApproved = isApproved,
                    Reason = reason
                };

                var response = await _httpClient.PutAsJsonAsync($"/api/users/{userId}/verification", request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating verification status for user: {UserId}", userId);
                throw;
            }
        }

        public async Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, string role)
        {
            try
            {
                var request = new { Email = email, Role = role };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/staff", request);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to create staff user. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to create staff user");
                }

                var result = await response.Content.ReadFromJsonAsync<CreateStaffUserResponse>();
                return (result.UserId, result.TempPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user with email: {Email}", email);
                throw;
            }
        }

        public async Task NotifyStaffUserAsync(Guid userId)
        {
            try
            {
                var response = await _httpClient.PostAsync($"/api/auth/staff/{userId}/notify", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to notify staff user. Status: {StatusCode}", response.StatusCode);
                    throw new Exception("Failed to notify staff user");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error notifying staff user: {UserId}", userId);
                throw;
            }
        }

        public async Task<List<UserDetailsDto>> GetIncompleteOnboardingUsersAsync()

        {

            try

            {

                var response = await _httpClient.GetAsync("/api/users/incomplete-onboarding");

                if (!response.IsSuccessStatusCode)

                {

                    _logger.LogError("Failed to fetch incomplete onboarding users. Status: {StatusCode}", response.StatusCode);

                    throw new Exception("Failed to fetch incomplete onboarding users from AuthService");

                }



                var users = await response.Content.ReadFromJsonAsync<List<UserDetailsDto>>();

                return users;

            }

            catch (Exception ex)

            {

                _logger.LogError(ex, "Error fetching incomplete onboarding users");

                throw;

            }

        }

        private class CreateStaffUserResponse
        {
            public Guid UserId { get; set; }
            public string TempPassword { get; set; }
        }
    }
} 