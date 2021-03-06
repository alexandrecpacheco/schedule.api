namespace Schedule.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public int UserProfileId { get; set; }
        public int ProfileId { get; set; }
        public int UserId { get; set; }
        public Profile Profile { get; set; } = new Profile();
    }
}
