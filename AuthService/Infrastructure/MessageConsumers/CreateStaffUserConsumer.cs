using AuthService.Application.Common.Interface.Services;
using AuthService.Application.DTO.Requests;
using MassTransit;
using SharedLibrary.Models.Messages;

public class CreateStaffUserConsumer : IConsumer<CreateStaffUserMessage>
{
    private readonly IAuthService _authService;
    private readonly ILogger<CreateStaffUserConsumer> _logger;

    public CreateStaffUserConsumer(
        IAuthService authService,
        ILogger<CreateStaffUserConsumer> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateStaffUserMessage> context)
    {
        try
        {
            var message = context.Message;
            _logger.LogInformation("Processing create staff user request for: {Email},{UserId} message.Email, message.InitiatorUserId  ");

            var userId = await _authService.CreateStaffUserAsync(new CreateStaffUserRequest
            {
                Email = message.Email,
                FirstName = message.FirstName,
                LastName = message.LastName,
                InitiatorUserId = message.InitiatorUserId
            });

            
            await context.RespondAsync(new CreateStaffUserResponse
            {
                UserId = userId
            });

           
            _logger.LogInformation("Staff user created successfully: {UserId}", userId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating staff user for email: {Email}");
            await context.RespondAsync(new CreateStaffUserResponse { UserId = Guid.Empty }); 
        }
    }
}