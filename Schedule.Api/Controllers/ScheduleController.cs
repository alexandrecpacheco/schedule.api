using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Schedule.Domain.Dto.Request;
using Schedule.Domain.Interfaces.Data.Service;
using System.Threading.Tasks;

namespace Schedule.Api.Controllers
{
    [Route("api/[controller]")]
    public class ScheduleController : BaseController
    {
        private readonly IScheduleService _scheduleService;
        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [Attributes.Authorize(Role.Admin, Role.Interviewer, Role.Candidate)]
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Create(ScheduleRequest request)
        {
            if (request == null) return BadRequest();

            await _scheduleService.Create(request);

            return await ResponseResult(true);
        }
    }
}
