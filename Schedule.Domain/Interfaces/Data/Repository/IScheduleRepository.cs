using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Repository
{
    public interface IScheduleRepository
    {
        Task<Domain.Entities.Schedule> GetScheduleAsync(DateTime startAt, DateTime endAt, int userProfileId);
        Task<int> Create(Entities.Schedule schedule, DbConnection dbConnection, DbTransaction dbTransaction);
    }
}
