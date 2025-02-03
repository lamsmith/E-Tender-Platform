namespace Backoffice_Services.Application.Features.UserManagement.Dtos
{
    public class UserVerificationDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CompanyName { get; set; }
        public bool IsVerified { get; set; }
        public string? VerificationStatus { get; set; }
    }
}