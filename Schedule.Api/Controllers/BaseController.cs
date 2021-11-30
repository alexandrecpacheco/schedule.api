using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Schedule.Domain;
using Schedule.Domain.Dto.Response;
using Schedule.Domain.Entities;
using Schedule.Domain.Utils;
using Schedule.Security;
using System;
using System.Threading.Tasks;

namespace Schedule.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BaseController : ControllerBase
    {
        protected Task<IActionResult> ResponseResult(object response)
        {
            try
            {
                IActionResult ok = Ok(new { success = true, data = response });
                return Task.FromResult(ok);
            }
            catch (Exception e)
            {
                return Task.FromResult<IActionResult>(BadRequest(new
                { success = false, errors = new[] { e.InnerException } }));
            }
        }

        protected static UserAuthenticatedResponse GenerateToken(User user, ApiSettings apiSettings)
        {
            var claims = user.UserProfile.Profile.Description;

            var tokenBuilder = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(apiSettings.Values.SecurityKey))
                .AddSubject("Authentication")
                .AddIssuer(apiSettings.Values.Issuer)
                .AddAudience(apiSettings.Values.Audience)
                .AddClaim("name", user.Name)
                .AddClaim("id", user.UserId.ToString())
                .AddClaimRole("role", string.Join(",", claims))
                .AddAlgorithm(SecurityAlgorithms.HmacSha256)
                .AddExpiry(apiSettings.Values.ExpirationTime);

            var token = tokenBuilder.Build();
            var refreshToken = Cryptography.SHA256(Guid.NewGuid().ToString());
            var userAuthenticatedResponse = new UserAuthenticatedResponse
            {
                Token = token.Value,
                RefreshToken = refreshToken,
                Name = user.Name,
                Email = user.Email
            };

            return userAuthenticatedResponse;
        }
    }
}
