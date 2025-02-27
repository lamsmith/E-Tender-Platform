using AuthService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO.Requests
{
    public class CreateStaffUserRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}