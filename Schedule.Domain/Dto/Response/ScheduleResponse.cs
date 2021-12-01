using System;

namespace Schedule.Domain.Dto.Response
{
    public class ScheduleResponse
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
    }
}
