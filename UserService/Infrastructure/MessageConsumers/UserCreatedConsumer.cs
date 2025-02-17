using MassTransit;
using Microsoft.Extensions.Logging;
using SharedLibrary.Models.Messages;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;

namespace UserService.Infrastructure.MessageConsumers
{
    public class UserCreatedConsumer : IConsumer<CreateUserMessage>
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(
            IUserService userService,
            ILogger<UserCreatedConsumer> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateUserMessage> context)
        {
            try
            {
                var message = context.Message;
                _logger.LogInformation("Received CreateUserMessage for user {UserId} with email {Email}",
                    message.UserId, message.Email);

                var request = new CompleteProfileRequest
                {
                    Email = message.Email,
                    FirstName = message.FirstName,
                    LastName = message.LastName
                };

                await _userService.CompleteProfileAsync(message.UserId, request);

                _logger.LogInformation("Successfully processed CreateUserMessage for user {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CreateUserMessage");
                throw;
            }
        }
    }
}