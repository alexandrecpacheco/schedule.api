using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule.Domain.Dto.Request;
using Schedule.Domain.Interfaces.Data.Service;
using System.Threading.Tasks;

namespace Schedule.Api.Controllers
{
    [Route("api/[controller]")]
    public class AuthorizationController : BaseController
    {
        private readonly IUserService _userService;

        public AuthorizationController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("authentication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Authentication([FromBody] SignInRequest request)
        {
            if (request == null) return BadRequest();

            var response = await _userService.Authentication(request);
            if (response == null) return await ResponseResult(false);
            var userAuthenticatedResponse = GenerateToken(response);

            return await ResponseResult(userAuthenticatedResponse);
        }

        [HttpGet("Test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public string Test()
        {
            return @"[{'Test': 'test', 'Second Test' : '2 test'}]";
        }
    }
}
