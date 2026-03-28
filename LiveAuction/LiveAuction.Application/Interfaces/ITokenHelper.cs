using LiveAuction.Application.Interfaces.RepositoryInterfaces.Read;
using LiveAuction.Application.Interfaces.RepositoryInterfaces.Write;
using LiveAuction.Core.Entites;
using LiveAuction.Core.Entites.AuthEntites;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace LiveAuction.Application.Interfaces
{
    public interface ITokenHelper
    {
        Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user, IList<string> roles);
        RefreshToken GenerateRefreshToken();
        Task ManageUserTokensAsync(IGenericWriteRepository<RefreshToken> _refreshTokenWriteRepo,
            IGenericReadRepository<RefreshToken> _refreshTokenReadRepo
            , Guid userId, CancellationToken cancellationToken);

        Task<string> GenerateRegisterToken(string email);

        Task<string?> ExtractEmailFromRegisterToken(string token);
    }
}
