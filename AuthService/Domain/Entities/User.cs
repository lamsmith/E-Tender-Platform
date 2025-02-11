using AuthService.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using SharedLibrary.Enums;
using AuthService.Domain.Common;

namespace AuthService.Domain.Entities
{
    public class User : BaseEntity
    {
        [Key]
   
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public Role Role { get; set; }
        public bool IsActive { get; set; }
        public bool RequirePasswordChange { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastPasswordChangeAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool EmailConfirmed { get; set; } = false;
        public AccountStatus Status { get; set; } = AccountStatus.Pending;
    }
}