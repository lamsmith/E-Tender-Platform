using UserService.Domain.Common;

namespace UserService.Domain.Entities
{
    public class CompanyLogo : BaseEntity
    {
        public string FileName { get; set; }
        public string FileUrl { get; set; }
        public string FileType { get; set; }
    }
}
