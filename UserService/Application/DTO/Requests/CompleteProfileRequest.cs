namespace UserService.Application.DTO.Requests
{
    public class CompleteProfileRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Industry { get; set; }
    }
}