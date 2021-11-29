using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.Api.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : Controller
    {
        //[Attributes.Authorize(Role.Admin)]
        //[HttpGet()]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> Get()
        ////{
        ////    if (request == null) return BadRequest();

        ////    var response = await _userService.Authentication(request);
        ////    if (response == null) return await ResponseResult(false);
        ////    var userAuthenticatedResponse = GenerateToken(response, _apiSettings);

        //    return await ResponseResult(userAuthenticatedResponse);
        //}
    }
}
