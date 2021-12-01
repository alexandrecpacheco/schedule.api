using Schedule.Domain.Dto.Response;
using Schedule.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Repository
{
    public interface IScheduleRepository
    {
        Task<Entities.Schedule> GetScheduleAsync(DateTime startAt, DateTime endAt, int userProfileId);
        Task<int> Create(Entities.Schedule schedule, DbConnection dbConnection, DbTransaction dbTransaction);
        Task<IList<ScheduleResponse>> GetSchedulesAsync(DateTime startAt, DateTime endAt, ProfileEnum profile);
    }
}
