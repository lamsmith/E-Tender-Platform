using SharedLibrary.Models.Messages;
using System.Security.Cryptography;
using System.Text;
using UserService.Application.Common.Interface.Repositories;
using UserService.Application.Common.Interface.Services;
using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;
using UserService.Domain.Paging;

namespace UserService.Infrastructure.Services
{
    public class UserServices : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserServices(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        

        public async Task<User> CompleteProfileAsync(Guid userId, CompleteProfileRequest request)
        {
            var user = await GetUserByIdAsync(userId);
            if (user.Profile != null)
            {
                throw new InvalidOperationException("Profile already completed");
            }

            // Create new profile for initial setup
            user.Profile = new Profile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
            };

            await _userRepository.UpdateAsync(user);


            return user;
        }


        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<int> GetUserCountAsync() => await _userRepository.GetCountAsync();


        public async Task<User> SubmitKycAsync(Guid userId, KycSubmissionRequest request)
        {
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User not found with ID: {userId}");
            }

            // Handle company logo upload if provided
            if (request.CompanyLogo != null)
            {
                // TODO: Implement file upload logic
                var companyLogo = new CompanyLogo
                {
                    FileName = request.CompanyLogo.FileName,
                    FileType = request.CompanyLogo.ContentType
                    // Set FileUrl after upload
                };
                user.Profile.CompanyLogo = companyLogo;
            }

            // Update KYC information using existing Profile fields
            user.Profile.CompanyName = request.CompanyName;
            user.Profile.PhoneNumber = request.PhoneNumber;
            user.Profile.CompanyAddress = request.CompanyAddress;
            user.Profile.Industry = request.Industry;
            user.Profile.State = request.State;
            user.Profile.City = request.City;

            await _userRepository.UpdateAsync(user);

            return user;

          
        }

        public async Task<PaginatedList<User>> GetIncompleteProfilesAsync(PageRequest pageRequest)
        {
            return await _userRepository.GetIncompleteProfilesAsync(pageRequest);
        }
    }
}
