using System;
using System.Collections.Generic;
using System.Text;

namespace Schedule.Domain.Entities
{
    public class UserProfile
    {
        public int UserProfileId { get; set; }
        public Profile Profile { get; set; } = new Profile();
    }
}
