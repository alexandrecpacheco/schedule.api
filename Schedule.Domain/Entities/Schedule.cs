using System;

namespace Schedule.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public int ScheduleId { get; set; }
        public string Description { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public UserProfile UserProfile { get; set; }
        public TaskSchedule TaskClass { get; set; }
    }
}
