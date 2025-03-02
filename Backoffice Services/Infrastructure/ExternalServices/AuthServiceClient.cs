using System.Net.Http.Json;
using Microsoft.Extensions.Configuration;
using MassTransit;
using SharedLibrary.Models.Messages;

namespace Backoffice_Services.Infrastructure.ExternalServices
{
    public class AuthServiceClient : IAuthServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthServiceClient> _logger;
        private readonly IRequestClient<CreateStaffUserMessage> _createStaffClient;

        public AuthServiceClient(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<AuthServiceClient> logger,
            IRequestClient<CreateStaffUserMessage> createStaffClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ExternalServices:AuthService:BaseUrl"]);
            _logger = logger;
            _createStaffClient = createStaffClient;
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

        public async Task<(Guid UserId, string TempPassword)> CreateStaffUserAsync(
            string email,
            string firstName,
            string lastName)
        {
            try
            {
                var message = new CreateStaffUserMessage
                {
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName
                };

                var response = await _createStaffClient.GetResponse<CreateStaffUserResponse>(message);
                var result = response.Message;

                _logger.LogInformation(
                    "Staff user created successfully in Auth Service. UserId: {UserId}, Email: {Email}",
                    result.UserId,
                    email);

                return (response.Message.UserId, "");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user in Auth Service. Email: {Email}", email);
                throw;
            }
        }

        public async Task<bool> UpdateAccountStatusAsync(Guid userId, SharedLibrary.Enums.AccountStatus newStatus, string? reason = null)
        {
            try
            {
                var request = new
                {
                    NewStatus = newStatus,
                    Reason = reason
                };

                var response = await _httpClient.PutAsJsonAsync($"api/users/{userId}/status", request);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation("Successfully updated account status for user {UserId} to {Status}",
                    userId, newStatus);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating account status for user {UserId} to {Status}",
                    userId, newStatus);
                throw;
            }
        }

        private class CreateStaffResponse
        {
            public Guid UserId { get; set; }
            public string TempPassword { get; set; }
        }
    }
}