namespace AuthService.Application.DTO.Responses
{
    public class UserDetailsDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
    }
}