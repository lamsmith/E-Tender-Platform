using MediatR;
using Backoffice_Services.Infrastructure.ExternalServices;
using Backoffice_Services.Application.Features.UserManagement.Queries;

namespace Backoffice_Services.Application.Features.UserManagement.Handlers
{
    public class GetIncompleteOnboardingTasksQueryHandler
        : IRequestHandler<GetIncompleteOnboardingTasksQuery, List<OnboardingTaskDto>>
    {
        private readonly IUserProfileServiceClient _userProfileClient;
        private readonly ILogger<GetIncompleteOnboardingTasksQueryHandler> _logger;

        public GetIncompleteOnboardingTasksQueryHandler(
            IUserProfileServiceClient userProfileClient,
            ILogger<GetIncompleteOnboardingTasksQueryHandler> logger)
        {
            _userProfileClient = userProfileClient;
            _logger = logger;
        }

        public async Task<List<OnboardingTaskDto>> Handle(
            GetIncompleteOnboardingTasksQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var userProfile = await _userProfileClient.GetUserProfileAsync(request.UserId);
                var tasks = new List<OnboardingTaskDto>();

                // Add tasks based on profile completion
                if (!userProfile.IsProfileCompleted)
                {
                    tasks.Add(new OnboardingTaskDto
                    {
                        TaskName = "Complete Profile",
                        Description = "Please complete your profile information",
                        OrderIndex = 1
                    });
                }

                // Add other tasks as needed...

                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incomplete onboarding tasks for user {UserId}", request.UserId);
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