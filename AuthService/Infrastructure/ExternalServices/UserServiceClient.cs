using System.Net.Http.Json;
using AuthService.Application.DTO.Responses;

namespace AuthService.Infrastructure.ExternalServices
{
    public interface IUserServiceClient
    {
        Task<string> GetUserRoleAsync(Guid userId);
        Task<UserDetailsDto> GetUserDetailsAsync(Guid userId);
    }

    public class UserServiceClient : IUserServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public UserServiceClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpClient.BaseAddress = new Uri(_configuration["ExternalServices:UserService:BaseUrl"]);
        }

        public async Task<string> GetUserRoleAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}/role");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch user role from UserService");
            }

            var role = await response.Content.ReadFromJsonAsync<string>();
            return role ?? "User";
        }

        public async Task<UserDetailsDto> GetUserDetailsAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/api/users/{userId}");
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to fetch user details from UserService");
            }

            var userDetails = await response.Content.ReadFromJsonAsync<UserDetailsDto>();
            return userDetails ?? throw new Exception("User details not found");
        }
    }
}