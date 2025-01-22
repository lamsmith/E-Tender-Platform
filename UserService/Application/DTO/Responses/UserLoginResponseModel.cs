namespace UserService.Application.DTO.Responses
{
    public class UserLoginResponseModel
    {
        public required string Firstname { get; set; }

        public required string LastName { get; set; }

        public required string Email { get; set; }

        public required string Token { get; set; }
    }
}
