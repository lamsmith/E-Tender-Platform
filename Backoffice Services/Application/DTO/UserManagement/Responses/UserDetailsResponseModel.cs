namespace Backoffice_Services.Application.DTO.UserManagement.Responses
{
    public class UserDetailsResponseModel
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public bool IsProfileCompleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string RcNumber { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string CompanyAddress { get; set; }
        public string Industry { get; set; }
        public Guid? CompanyLogoId { get; set; }
        public string? CompanyLogoUrl { get; set; }
    }
}
