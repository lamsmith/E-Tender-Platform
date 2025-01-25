namespace Backoffice_Services.Domain.Entities
{
    public class Staff
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; } // e.g., Super Admin, Reviewer
        public bool IsActive { get; set; }
    }
}
