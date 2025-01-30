using MediatR;
using AuthService.Application.Common.Interface.Services;
using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;

namespace AuthService.Application.Features.Handlers
{
    public class SendStaffWelcomeEmailCommandHandler : IRequestHandler<SendStaffWelcomeEmailCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly ILogger<SendStaffWelcomeEmailCommandHandler> _logger;

        public SendStaffWelcomeEmailCommandHandler(
            IUserRepository userRepository,
            IMessagePublisher messagePublisher,
            ILogger<SendStaffWelcomeEmailCommandHandler> logger)
        {
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
            _logger = logger;
        }

        public async Task Handle(SendStaffWelcomeEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                    throw new Exception("User not found");

                var message = new StaffWelcomeEmailMessage
                {
                    Email = user.Email,
                    ResetPasswordUrl = $"https://your-domain.com/reset-password?token={user.Id}"
                };

                _messagePublisher.PublishMessage(MessageQueues.Notifications, message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending welcome email for user: {UserId}", request.UserId);
                throw;
            }
        }
    }
}