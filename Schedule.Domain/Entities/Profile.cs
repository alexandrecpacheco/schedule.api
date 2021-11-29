using System.Collections.Generic;

namespace Schedule.Domain.Entities
{
    public class Profile : BaseEntity
    {
        public Profile()
        {
            UserProfileCollection = new HashSet<UserProfile>();
        }

        public int ProfileId { get; set; }
        public string Description { get; set; }
        public ICollection<UserProfile> UserProfileCollection { get; set; }
    }
}
