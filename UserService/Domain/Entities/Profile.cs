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
        public string RcNumber { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string CompanyAddress { get; set; }
        public string Industry { get; set; }
        public Guid? CompanyLogoId { get; set; }
        public virtual CompanyLogo? CompanyLogo { get; set; }
        public virtual User User { get; set; }
    }
}
