using MediatR;

namespace Backoffice_Services.Application.Features.UserManagement.Queries
{
    public class GetIncompleteOnboardingTasksQuery : IRequest<List<OnboardingTaskDto>>
    {
        public Guid UserId { get; set; }
    }

    public class OnboardingTaskDto
    {
        public string TaskName { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? RequiredRole { get; set; }
        public int OrderIndex { get; set; }
    }
}