using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        protected static UserAuthenticatedResponse GenerateToken(User user)
        {
            var claims = user.UserProfile.Profile.Description;

            //var securityKey = System.Environment.GetEnvironmentVariable("QueueName");
            //var issuer = System.Environment.GetEnvironmentVariable("QueueName");
            //var audience = System.Environment.GetEnvironmentVariable("QueueName");
            //var expireInHour = Convert.ToDouble(System.Environment.GetEnvironmentVariable("QueueName"));
            var securityKey = "knD2&G|/fae0I1@iP64l{>2+jL7UNF1Tb<|P`|2.q2}Qpsr$M2Bb71FPm45F]G";
            var issuer = "http://localhost";
            var audience = "Schedule Api";
            var expireInHour = 1;

            var tokenBuilder = new JwtTokenBuilder()
                .AddSecurityKey(JwtSecurityKey.Create(securityKey))
                .AddSubject("Authentication")
                .AddIssuer(issuer)
                .AddAudience(audience)
                .AddClaim("name", user.Name)
                .AddClaim("id", user.UserId.ToString())
                .AddClaimRole("role", string.Join(",", claims))
                .AddAlgorithm(SecurityAlgorithms.HmacSha256)
                .AddExpiry(expireInHour);

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
