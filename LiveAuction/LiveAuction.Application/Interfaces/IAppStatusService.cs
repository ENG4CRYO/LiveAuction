using LiveAuction.Application.Common;
using LiveAuction.Application.Dtos.AppStatusModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiveAuction.Application.Interfaces
{
    public interface IAppStatusService
    {
        Task<ApiResponse<AppStatusResult>> CheckStatusAsync(string clientVersion, string os);
    }
}
