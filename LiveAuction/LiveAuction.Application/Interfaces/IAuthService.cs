using System;
using System.Collections.Generic;
using System.Text;
using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AuthModel;

namespace LiveAuction.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthModel>> RegisterAsync(RegisterModel model, CancellationToken cancellationToken);
        Task<ApiResponse<AuthModel>> GetTokenAsync(TokenRequestModel model, CancellationToken cancellationToken);
        Task<ApiResponse<AuthModel>> RefreshTokenAsync(string token, CancellationToken cancellationToken);
        Task<ApiResponse<bool>> RevokeTokenAsync(string token, CancellationToken cancellationToken);
        Task<ApiResponse<string>> RequestOtpAsync(OtpRequestModel model, CancellationToken cancellationToken);
        Task<ApiResponse<string>> VerifyOtpAsync(OtpVerifyModel model, CancellationToken cancellationToken);
    }
}
