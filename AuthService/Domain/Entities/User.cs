using System.ComponentModel.DataAnnotations;

namespace AuthService.Domain.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }    
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; } = true;
        public bool EmailConfirmed { get; set; } = false;
    }
}