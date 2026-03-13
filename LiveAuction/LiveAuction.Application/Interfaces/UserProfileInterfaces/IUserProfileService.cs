using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AuthModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Interfaces.UserProfileInterfaces
{
    public interface IUserProfileService
    {
        Task<ApiResponse<ProfileRequestDto>> GetUserProfileAsync(Guid id, CancellationToken cancellationToken);
        
    }
}
