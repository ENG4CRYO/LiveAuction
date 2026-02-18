using System;
using System.Collections.Generic;
using System.Text;
using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AuthModel;

namespace LiveAuction.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthModel>> RegisterAsync(RegisterModel model);
        Task<ApiResponse<AuthModel>> GetTokenAsync(TokenRequestModel model);
        Task<ApiResponse<AuthModel>> RefreshTokenAsync(string token);
        Task<ApiResponse<bool>> RevokeTokenAsync(string token);
        Task<ApiResponse<string>> RequestOtpAsync(OtpRequestModel model);
        Task<ApiResponse<string>> VerifyOtpAsync(OtpVerifyModel model);
    }
}
