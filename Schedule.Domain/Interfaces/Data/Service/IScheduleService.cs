using Schedule.Domain.Dto.Request;
using Schedule.Domain.Dto.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Service
{
    public interface IScheduleService
    {
        Task Create(ScheduleRequest schedule);
        Task<IEnumerable<ScheduleResponse>> GetSchedulesAsync(string name, string profile, DateTime startAt, DateTime endAt);
    }
}
