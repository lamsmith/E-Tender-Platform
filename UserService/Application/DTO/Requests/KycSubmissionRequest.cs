namespace UserService.Application.DTO.Requests
{
    public class KycSubmissionRequest
    {
        public string CompanyName { get; set; }
        public string RcNumber { get; set; }  
        public string CompanyAddress { get; set; }
        public string Industry { get; set; }
        public string PhoneNumber { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public IFormFile CompanyLogo { get; set; }
    }
}