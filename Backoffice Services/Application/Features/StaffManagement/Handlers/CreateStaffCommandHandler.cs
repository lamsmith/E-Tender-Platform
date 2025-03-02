using Backoffice_Services.Application.Common.Interfaces;
using Backoffice_Services.Application.Features.Commands;
using Backoffice_Services.Domain.Entities;
using MassTransit;
using MediatR;
using SharedLibrary.Models.Messages;

public class CreateStaffCommandHandler : IRequestHandler<CreateStaffCommand, Guid>
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IStaffRepository _staffRepository;
    private readonly ILogger<CreateStaffCommandHandler> _logger;
    private readonly IRequestClient<CreateStaffUserMessage> _requestClient;

    public CreateStaffCommandHandler(
        IPublishEndpoint publishEndpoint,
        IStaffRepository staffRepository,
        IRequestClient<CreateStaffUserMessage> requestClient,
        ILogger<CreateStaffCommandHandler> logger)
    {
        _publishEndpoint = publishEndpoint;
        _staffRepository = staffRepository;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateStaffCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting staff creation process for: {Email}", request.Email);

            var response = await _requestClient.GetResponse<CreateStaffUserResponse>(new CreateStaffUserMessage
            {
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                InitiatorUserId = request.InitiatorUserId
            }, cancellationToken);

            var userId = response.Message.UserId;

            if (userId == Guid.Empty)
            {
                _logger.LogWarning("Received invalid UserId from AuthService for email: {Email}", request.Email);
                throw new Exception("Failed to receive valid UserId from AuthService");
            }

            _logger.LogInformation("Received UserId from AuthService: {UserId}. Creating staff record...", userId);

            var staff = new Staff
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.InitiatorUserId,
                PhoneNumber = request.PhoneNumber,
                IsActive = true 
            };

            await _staffRepository.CreateAsync(staff);

            _logger.LogInformation("Staff record created successfully. StaffId: {StaffId}, UserId: {UserId}", staff.Id, userId);

            return staff.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in staff creation process for email: {Email}", request.Email);
            throw;
        }
    }
}