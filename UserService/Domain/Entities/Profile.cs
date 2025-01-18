using UserService.Domain.Common;

namespace UserService.Domain.Entities
{
    public class Profile : BaseEntity
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public User User { get; set; }
    }
}
