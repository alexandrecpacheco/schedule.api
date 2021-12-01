using System;

namespace Schedule.Domain.Dto.Request
{
    public class ScheduleRequest 
    {
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
