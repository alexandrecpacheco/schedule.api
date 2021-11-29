namespace Schedule.Domain.Entities
{
    public class TaskSchedule : BaseEntity
    {
        public int TaskId { get; set; }
        public string Description { get; set; }
        public Schedule Schedule { get; set; }
    }
}
