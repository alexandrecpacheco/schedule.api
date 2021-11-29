using Schedule.Domain.Dto.Request;
using Schedule.Domain.Entities;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using Schedule.Domain.Utils;
using System.Threading.Tasks;

namespace Schedule.Api.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDatabase _database;
        public UserService(IUserRepository userRepository, IDatabase database)
        {
            _userRepository = userRepository;
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
    }
}
