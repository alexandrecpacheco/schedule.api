using System;
using System.Collections.Generic;

namespace Schedule.Domain.Dto.Response
{
    public class ScheduleResponse
    {
        public ScheduleResponse()
        {
            SlotTime = new HashSet<DateTime>();
        }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Profile { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public ICollection<DateTime> SlotTime { get; set; }
    }
}
