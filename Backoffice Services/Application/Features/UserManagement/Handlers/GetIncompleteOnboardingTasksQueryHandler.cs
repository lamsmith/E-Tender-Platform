using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Application.Features.UserManagement.Queries;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class GetIncompleteOnboardingTasksQueryHandler
        : IRequestHandler<GetIncompleteOnboardingTasksQuery, List<OnboardingTaskDto>>
    {
        private readonly IAuthServiceClient _authServiceClient;
        private readonly ILogger<GetIncompleteOnboardingTasksQueryHandler> _logger;

        public GetIncompleteOnboardingTasksQueryHandler(
            IAuthServiceClient authServiceClient,
            ILogger<GetIncompleteOnboardingTasksQueryHandler> logger)
        {
            _authServiceClient = authServiceClient;
            _logger = logger;
        }

        public async Task<List<OnboardingTaskDto>> Handle(
            GetIncompleteOnboardingTasksQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userDetails = await _authServiceClient.GetUserDetailsAsync(request.UserId);
                var tasks = new List<OnboardingTaskDto>();

                // Check email verification
                if (!userDetails.EmailConfirmed)
                {
                    tasks.Add(new OnboardingTaskDto
                    {
                        TaskName = "Email Verification",
                        Description = "Verify your email address to activate your account",
                        IsCompleted = false,
                        OrderIndex = 1
                    });
                }

                // Add role-specific tasks
                switch (userDetails.Role?.ToLower())
                {
                    case "supplier":
                        tasks.AddRange(GetSupplierTasks());
                        break;
                    case "buyer":
                        tasks.AddRange(GetBuyerTasks());
                        break;
                }

                return tasks.OrderBy(t => t.OrderIndex).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incomplete onboarding tasks for user: {UserId}", request.UserId);
                throw;
            }
        }

        private List<OnboardingTaskDto> GetSupplierTasks()
        {
            return new List<OnboardingTaskDto>
            {
                new()
                {
                    TaskName = "Company Information",
                    Description = "Complete your company profile with business details",
                    RequiredRole = "Supplier",
                    OrderIndex = 2
                },
                new()
                {
                    TaskName = "Document Verification",
                    Description = "Upload required business documents for verification",
                    RequiredRole = "Supplier",
                    OrderIndex = 3
                }
            };
        }

        private List<OnboardingTaskDto> GetBuyerTasks()
        {
            return new List<OnboardingTaskDto>
            {
                new()
                {
                    TaskName = "Organization Details",
                    Description = "Complete your organization profile",
                    RequiredRole = "Buyer",
                    OrderIndex = 2
                }
            };
        }
    }
}