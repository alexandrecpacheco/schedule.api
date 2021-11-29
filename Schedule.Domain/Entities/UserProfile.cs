namespace Schedule.Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public int UserProfileId { get; set; }
        public Profile Profile { get; set; } = new Profile();
    }
}
