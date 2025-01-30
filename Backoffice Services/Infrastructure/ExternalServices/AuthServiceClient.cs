using System.Net.Http.Json;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthServiceClient> _logger;

        public AuthServiceClient(
            HttpClient httpClient,
            ILogger<AuthServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(string email, string role)
        {
            try
            {
                var request = new { Email = email, Role = role };
                var response = await _httpClient.PostAsJsonAsync("/api/auth/staff", request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to create staff user: {error}");
                }

                var result = await response.Content.ReadFromJsonAsync<CreateStaffUserResponse>();
                return (result.UserId, result.TempPassword);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Auth Service to create staff user");
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
                    var error = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to notify staff user: {error}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Auth Service to notify staff user");
                throw;
            }
        }
    }

    public class CreateStaffUserResponse
    {
        public Guid UserId { get; set; }
        public string TempPassword { get; set; }
    }
}