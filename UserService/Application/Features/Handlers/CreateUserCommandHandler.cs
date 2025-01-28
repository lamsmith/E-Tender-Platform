using MediatR;
using Microsoft.Extensions.Logging;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Features.Commands;
using UserService.Domain.Entities;

namespace UserService.Application.Features.Handlers
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(
            IUserRepository userRepository,
            ILogger<CreateUserCommandHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new User
                {
                    UserId = request.UserId,
                    Email = request.Email,
                    Profile = request.Profile?.Select(doc => new Profile
                    {
                        UserId = request.UserId,
                        FirstName = doc.FirstName,
                        LastName = doc.LastName,
                        CompanyName = doc.CompanyName,
                        PhoneNumber = doc.PhoneNumber,
                        Address = doc.Address
                    }).FirstOrDefault() 
                };

                await _userRepository.AddAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user with ID: {UserId}", request.UserId);
                throw;
            }
        }
    }
}