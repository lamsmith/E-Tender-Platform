using AuthService.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Application.DTO.Requests
{
    public class CreateStaffUserRequest
    {
        [EmailAddress]
        public string Email { get; set; }
    }
}