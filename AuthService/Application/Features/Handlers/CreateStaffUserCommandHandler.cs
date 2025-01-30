using MediatR;
using AuthService.Application.Common.Interface.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;

namespace AuthService.Application.Features.Handlers
{
    public class CreateStaffUserCommandHandler : IRequestHandler<CreateStaffUserCommand, CreateStaffUserResponse>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly IPasswordGenerator _passwordGenerator;
        private readonly ILogger<CreateStaffUserCommandHandler> _logger;

        public CreateStaffUserCommandHandler(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            IPasswordGenerator passwordGenerator,
            ILogger<CreateStaffUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _passwordGenerator = passwordGenerator;
            _logger = logger;
        }

        public async Task<CreateStaffUserResponse> Handle(CreateStaffUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await _userRepository.EmailExistsAsync(request.Email))
                    throw new Exception("Email already exists");

                var tempPassword = _passwordGenerator.GenerateTemporaryPassword();

                var user = new User
                {
                    Email = request.Email,
                    PasswordHash = _passwordHashService.HashPassword(tempPassword),
                    Role = request.Role,
                    IsActive = true,
                    RequirePasswordChange = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.CreateAsync(user);

                return new CreateStaffUserResponse
                {
                    UserId = user.Id,
                    TempPassword = tempPassword
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff user");
                throw;
            }
        }
    }
}