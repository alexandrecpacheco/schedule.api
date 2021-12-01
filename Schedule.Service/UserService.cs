using Schedule.Domain.Dto.Request;
using Schedule.Domain.Entities;
using Schedule.Domain.Enums;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using Schedule.Domain.Utils;
using Serilog;
using System;
using System.Data;
using System.Threading.Tasks;

namespace Schedule.Api.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IDatabase _database;
        public UserService(IUserRepository userRepository, 
            IDatabase database,
            IUserProfileRepository userProfileRepository)
        {
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _database = database;
        }

        public async Task<User> Authentication(SignInRequest signInRequest)
        {
            var user = new User()
            {
                Email = signInRequest.Email,
                Password = signInRequest.Password
            };

            signInRequest.Password = Cryptography.SHA256(signInRequest.Password);

            var currentUser = await _userRepository.GetByEmailAndPassword(user);
            if (currentUser == null) return null;

            if (currentUser.IsActive == false) return null;

            return currentUser;
        }

        public async Task Create(UserRequest userRequest)
        {
            var emailAvailable = await GetByEmail(userRequest.Email);
            if (emailAvailable != null)
            {
                Log.Information("Email is already taken");
                throw new DuplicateNameException($"Email {userRequest.Email} is already taken");
            }

            await _database.ExecuteInTransaction(async (connection, transaction) =>
            {
                var user = new User
                {
                    Name = userRequest.Name.Trim(),
                    Email = userRequest.Email.Trim(),
                    Password = Cryptography.SHA256(userRequest.Password.Trim()),
                    IsActive = userRequest.IsActive
                };

                Log.Information("Insert a new user");
                var userId = await _userRepository.Create(user, connection, transaction);
                var profile = (ProfileEnum)Enum.Parse(typeof(ProfileEnum), userRequest.UserProfile.Profile.Description);
                var userProfile = new UserProfile
                {
                    ProfileId = (int)profile,
                    UserId = userId
                };

                Log.Information("Create a user profile");
                await _userProfileRepository.Create(userProfile, connection, transaction);
            });
        }

        public async Task<User> GetByEmail(string email)
        {
            return await _userRepository.GetByEmail(email);
        }
    }
}
