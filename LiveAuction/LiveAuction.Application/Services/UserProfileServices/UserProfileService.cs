using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.UserProfileDtos;
using LiveAuction.Application.Interfaces.RepositoryInterfaces.Read;
using LiveAuction.Application.Interfaces.UserProfileInterfaces;
using LiveAuction.Core.Entites;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Services.UserProfileServices
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IAuctionReadRepository _auctionReadRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileService(IAuctionReadRepository auctionReadRepository,
            UserManager<ApplicationUser> userManger) 
        {
            _auctionReadRepository = auctionReadRepository;
            _userManager = userManger;
        }
        public async Task<ApiResponse<ProfileRequestDto>> GetUserProfileAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _auctionReadRepository.GetUserProfileAsync(id, cancellationToken);
            if (!result.HasValue)
            {
                return ApiResponse<ProfileRequestDto>.Failure("User profile not found.");
            }

            var (userEntity, totalItems, soldItems, avgPrice) = result.Value;

            var userProfileDto = new ProfileRequestDto
            {
                Id = userEntity.Id,
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                ProfilePictureUrl = userEntity.ProfilePictureUrl ?? string.Empty,
                Bio = userEntity.Bio ?? string.Empty,

                TotalItems = totalItems,
                SoldItems = soldItems,
                AvgPrice = avgPrice
            };

            return ApiResponse<ProfileRequestDto>.Success(userProfileDto, "User profile retrieved successfully.");
        }
    }
}
