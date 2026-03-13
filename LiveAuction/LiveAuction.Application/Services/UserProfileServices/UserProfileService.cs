using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AuthModel;
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
            var userProfile = await _auctionReadRepository.GetUserProfileAsync(id, cancellationToken);

            if (userProfile == null)
            {
                return ApiResponse<ProfileRequestDto>.Failure("User profile not found.");
            }

            return ApiResponse<ProfileRequestDto>.Success(userProfile, "User profile retrieved successfully.");
        }
    }
}
