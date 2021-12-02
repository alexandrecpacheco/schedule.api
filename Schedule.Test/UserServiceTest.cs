using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Schedule.Api.Service;
using Schedule.Domain.Dto.Request;
using Schedule.Domain.Entities;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using System.Threading.Tasks;

namespace Schedule.Test
{
    [TestClass]
    public class UserServiceTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepository;
        private readonly Mock<IDatabase> _mockIDatabase;
        public UserServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockIDatabase = new Mock<IDatabase>();
            _mockUserProfileRepository = new Mock<IUserProfileRepository>();
        }

        [TestMethod]
        public async Task UserService_GetByEmail_ReturnsAUser()
        {
            string email = "admin@gmail.com";

            var user = new User()
            {
                Name = "Test",
                Email = "email@test.com"
            };

            _mockUserRepository.Setup(c => c.GetByEmail(It.IsAny<string>()))
                .Returns(Task.FromResult(user));

            _mockUserService.Setup(s => s.GetByEmail(It.IsAny<string>()))
                .Returns(Task.FromResult(It.IsAny<User>()));

            var userService = new UserService(_mockUserRepository.Object, _mockIDatabase.Object,
                _mockUserProfileRepository.Object);

            var expected = await userService.GetByEmail(email);

            Assert.IsTrue(!string.IsNullOrWhiteSpace(expected.Name));
        }

        [TestMethod]
        public async Task UserService_Authentication_ReturnsAUser()
        {
            var signInRequest = new SignInRequest()
            {
                Email = "test@test.com",
                Password = "password"
            };

            var user = new User()
            {
                Name = "Test",
                Email = "testUser@test.com",
                IsActive = true
            };

            _mockUserRepository.Setup(c => c.GetByEmailAndPassword(It.IsAny<User>()))
                .Returns(Task.FromResult(user));

            var userService = new UserService(_mockUserRepository.Object,
                _mockIDatabase.Object, _mockUserProfileRepository.Object);

            var expected = await userService.Authentication(signInRequest);

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public async Task UserService_Authentication_ReturnsUserIsNotActivated()
        {
            var signInRequest = new SignInRequest()
            {
                Email = "test@test.com",
                Password = "password"
            };

            var user = new User()
            {
                Name = "Test",
                Email = "testUser@test.com"
            };

            _mockUserRepository.Setup(c => c.GetByEmailAndPassword(It.IsAny<User>()))
                .Returns(Task.FromResult(user));

            var userService = new UserService(_mockUserRepository.Object,
                _mockIDatabase.Object, _mockUserProfileRepository.Object);

            var expected = await userService.Authentication(signInRequest);

            Assert.IsNull(expected);
        }
    }
}
