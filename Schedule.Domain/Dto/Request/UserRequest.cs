namespace Schedule.Domain.Dto.Request
{
    public class UserRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public UserProfileRequest UserProfile { get; set; }
    }

    public class UserProfileRequest
    {
        public ProfileRequest Profile { get; set; }
    }

    public class ProfileRequest
    {
        public string Description { get; set; }
    }
}
