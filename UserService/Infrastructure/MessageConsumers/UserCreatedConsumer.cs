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
                _logger.LogInformation("Creating user profile for {UserId}", message.UserId);

                var profileRequest = new CompleteProfileRequest
                {
                    FirstName = message.FirstName,
                    LastName = message.LastName
                };

                await _userService.CompleteProfileAsync(message.UserId, profileRequest);

                _logger.LogInformation("User profile created successfully for {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile for {UserId}", context.Message.UserId);
                throw;
            }
        }
    }
}