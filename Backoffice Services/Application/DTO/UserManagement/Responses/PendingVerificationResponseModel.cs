namespace Backoffice_Services.Application.DTO.UserManagement.Responses
{
    public class PendingVerificationResponseModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
