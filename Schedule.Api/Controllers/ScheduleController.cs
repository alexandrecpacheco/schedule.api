using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Schedule.Domain.Dto.Request;
using Schedule.Domain.Interfaces.Data.Service;
using System;
using System.ComponentModel.DataAnnotations;
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

        public string Name { get; set; }
        public string Profile { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        [Attributes.Authorize(Role.Admin, Role.Interviewer, Role.Candidate)]
        [HttpGet("search-schedule/{name}/{profile}/{startAt}/{endAt}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSchedule([BindRequired] string name, [BindRequired] string profile, 
            [BindRequired] DateTime startAt, [BindRequired] DateTime endAt)
        {
            if (!ModelState.IsValid) return BadRequest();

            var search = await _scheduleService.GetSchedulesAsync(name, profile, startAt, endAt);

            return await ResponseResult(search);
        }
    }
}
