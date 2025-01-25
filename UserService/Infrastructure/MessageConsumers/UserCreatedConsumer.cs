using SharedLibrary.MessageBroker;
using SharedLibrary.Models.Messages;
using UserService.Application.Common.Interface.Services;
using UserService.Domain.Enums;

namespace UserService.Infrastructure.MessageConsumers
{
    public class UserCreatedConsumer : IMessageConsumer<CreateUserMessage>
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(IUserService userService, ILogger<UserCreatedConsumer> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task ConsumeAsync(CreateUserMessage message)
        {
            try
            {
                _logger.LogInformation("Creating user profile for {UserId}", message.UserId);

                await _userService.CreateUserAsync(new CreateUserRequest
                {
                    UserId = message.UserId.ToString(),
                    Email = message.Email,
                    Role = Role.User, // Default role from your enum
                    Profile = new Domain.Entities.Profile() // Initialize with default values
                });

                _logger.LogInformation("User profile created successfully for {UserId}", message.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user profile for {UserId}", message.UserId);
                throw;
            }
        }
    }
}