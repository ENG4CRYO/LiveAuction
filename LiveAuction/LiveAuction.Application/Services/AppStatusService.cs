using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AppStatusModel;
using LiveAuction.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LiveAuction.Application.Services
{
    public class AppStatusService : IAppStatusService
    {
        private readonly IConfiguration _configuration;

        public AppStatusService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<ApiResponse<AppStatusResult>> CheckStatusAsync(string clientVersion, string? os = null)
        {
            os = os?.ToLower();

            if (!string.IsNullOrEmpty(os) && os != "android" && os != "ios")
            {
                return CreateErrorResponse("Unsupported operating system. Only 'android' and 'ios' are allowed.");
            }

            if (!Version.TryParse(clientVersion, out var currentVersion))
            {
                return CreateErrorResponse("Invalid client version format.");
            }

            var maintenanceResult = await CheckMaintenanceAsync();
            if (maintenanceResult != null)
                return CreateSuccessResponse(maintenanceResult, "System is under maintenance.");

            var bannedResult = await CheckBannedVersionAsync(clientVersion,os);
            if (bannedResult != null)
                return CreateSuccessResponse(bannedResult, "Version is banned.");

            var updateResult = await CheckUpdateRequiredAsync(currentVersion, os);
            if (updateResult != null)
                return CreateSuccessResponse(updateResult, "Update required.");

            var okResult = new AppStatusResult
            {
                IsMaintenance = false,
                UpdateRequired = false,
                IsBanned = false,
                Message = "System is operational."
            };

            return CreateSuccessResponse(okResult, "Success.");
        }

        #region Private Helper Methods
        private async Task<AppStatusResult?> CheckMaintenanceAsync()
        {
            await Task.CompletedTask; 

            if (_configuration.GetValue<bool>("AppStatus:IsMaintenanceMode"))
            {
                return new AppStatusResult
                {
                    IsMaintenance = true,
                    Message = _configuration.GetValue<string>("AppStatus:MaintenanceMessage")
                              ?? "The system is currently under maintenance. Please try again later."
                };
            }
            return null;
        }

        private async Task<AppStatusResult?> CheckBannedVersionAsync(string clientVersion,string? os = null)
        {
            await Task.CompletedTask;

            var bannedVersionsConfig = _configuration.GetValue<string>("AppStatus:BannedVersions") ?? "";
            var bannedList = bannedVersionsConfig
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(v => v.Trim());

            if (bannedList.Contains(clientVersion))
            {
                string storeUrl = string.Empty;
                if (os == "android") storeUrl = _configuration.GetValue<string>("AppStatus:StoreUrlAndroid") ?? "";
                else if (os == "ios") storeUrl = _configuration.GetValue<string>("AppStatus:StoreUrliOS") ?? "";
                return new AppStatusResult
                {
                    
                    IsBanned = true,
                    Message = _configuration.GetValue<string>("AppStatus:BannedMessage")
                              ?? "This version is no longer supported due to technical issues. Please update your app.",
                    StoreUrl = storeUrl
                };
            }
            return null;
        }

        private async Task<AppStatusResult?> CheckUpdateRequiredAsync(Version currentVersion, string? os)
        {
            await Task.CompletedTask;

            var minimumVersionStr = _configuration.GetValue<string>("AppStatus:GlobalMinVersion") ?? "1.0.0";

            if (Version.TryParse(minimumVersionStr, out var minimumVersion) && currentVersion < minimumVersion)
            {
                string storeUrl = string.Empty;
                if (os == "android") storeUrl = _configuration.GetValue<string>("AppStatus:StoreUrlAndroid") ?? "";
                else if (os == "ios") storeUrl = _configuration.GetValue<string>("AppStatus:StoreUrliOS") ?? "";

                return new AppStatusResult
                {
                    UpdateRequired = true,
                    Message = _configuration.GetValue<string>("AppStatus:UpdateRequiredMessage")
                              ?? "A new mandatory update is available. Please update your app to continue.",
                    StoreUrl = storeUrl
                };
            }
            return null;
        }

        private ApiResponse<AppStatusResult> CreateSuccessResponse(AppStatusResult data, string message)
        {
            return new ApiResponse<AppStatusResult>
            {
                Succeeded = true,
                Data = data,
                Message = message
            };
        }

        private ApiResponse<AppStatusResult> CreateErrorResponse(string errorMessage)
        {
            return new ApiResponse<AppStatusResult>
            {
                Succeeded = false,
                Message = errorMessage,
                Data = null!
            };
        }
        #endregion
    }
}