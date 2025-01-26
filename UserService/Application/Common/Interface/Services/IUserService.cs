﻿using UserService.Application.DTO.Requests;
using UserService.Domain.Entities;

namespace UserService.Application.Common.Interface.Services
{
    public interface IUserService
    {
        Task<User> UpdateProfileAsync(Guid userId, UserProfileUpdateRequestModel profileUpdate);
        //Task ChangePasswordAsync(Guid userId, UserChangePasswordRequestModel changePassword);
        Task<int> GetUserRfqCreatedCountAsync(Guid userId);
        Task<int> GetUserBidsSubmittedCountAsync(Guid userId);
        Task<decimal> GetUserBidSuccessRateAsync(Guid userId);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<int> GetUserCountAsync();

    }
}
