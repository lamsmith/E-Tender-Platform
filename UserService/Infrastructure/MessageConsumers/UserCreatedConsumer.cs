using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Infrastructure.MessageBrokers;
using SharedLibrary.Models.Messages;
using UserService.Application.Features.Commands;
namespace UserService.Infrastructure.MessageConsumers
{
    public class UserCreatedConsumer : IMessageConsumer<CreateUserMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UserCreatedConsumer> _logger;

        public UserCreatedConsumer(
            IMediator mediator,
            ILogger<UserCreatedConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task ConsumeAsync(CreateUserMessage message)
        {
            try
            {
                _logger.LogInformation("Creating user profile for {UserId}", message.UserId);

                var command = new CreateUserCommand
                {
                    UserId = message.UserId,
                    Email = message.Email,
                    CreatedAt = message.CreatedAt
                };

                await _mediator.Send(command);

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