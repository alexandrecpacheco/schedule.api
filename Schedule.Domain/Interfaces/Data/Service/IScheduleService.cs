using Schedule.Domain.Dto.Request;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Service
{
    public interface IScheduleService
    {
        Task Create(ScheduleRequest schedule);
    }
}
