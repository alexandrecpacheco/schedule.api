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
        
        [HttpGet("Test")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public string Test()
        {
            return @"[{'Test': 'test', 'Second Test' : '2 test'}]";
        }
    }
}
