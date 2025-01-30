using MediatR;
using AuthService.Application.Common.Interface.Services;

namespace AuthService.Application.Features.Handlers
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHashService _passwordHashService;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            IUserRepository userRepository,
            IPasswordHashService passwordHashService,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userRepository = userRepository;
            _passwordHashService = passwordHashService;
            _logger = logger;
        }

        public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                    throw new Exception("User not found");

                if (!_passwordHashService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
                    throw new Exception("Current password is incorrect");

                user.PasswordHash = _passwordHashService.HashPassword(request.NewPassword);
                user.RequirePasswordChange = false;
                user.LastPasswordChangeAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user: {UserId}", request.UserId);
                throw;
            }
        }
    }
}