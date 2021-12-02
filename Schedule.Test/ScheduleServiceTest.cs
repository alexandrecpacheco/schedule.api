using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Schedule.Domain.Dto.Request;
using Schedule.Domain.Dto.Response;
using Schedule.Domain.Entities;
using Schedule.Domain.Enums;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using Schedule.Service;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Schedule.Test
{
    [TestClass]
    public class ScheduleServiceTest
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserProfileRepository> _mockUserProfileRepository;
        private readonly Mock<IScheduleRepository> _mockScheduleRepository;
        private readonly Mock<IDatabase> _mockIDatabase;
        public ScheduleServiceTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockIDatabase = new Mock<IDatabase>();
            _mockUserProfileRepository = new Mock<IUserProfileRepository>();
            _mockScheduleRepository = new Mock<IScheduleRepository>();
        }

        [TestMethod]
        public async Task ScheduleService_GetScheduleAsync_ReturnsASchedule()
        {
            var userProfile = new UserProfile()
            {
                UserProfileId = 1,
                Profile = new Profile()
                {
                    Description = "Interviewer"
                },
                UserId = 1
            };
            _mockUserProfileRepository.Setup(m => m.GetUserProfile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(userProfile));


            var schedule = new Domain.Entities.Schedule()
            {
                Description = "Schedule",
                UserProfileId = 1
            };
            _mockScheduleRepository.Setup(s => s.GetScheduleAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<int>()))
                .Returns(Task.FromResult(schedule));

            var request = new ScheduleRequest()
            {
                Name = "Test",
                Email = "email@test.com",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now
            };


            var scheduleService = new ScheduleService(_mockIDatabase.Object, _mockUserProfileRepository.Object, _mockScheduleRepository.Object);
            var expected = await scheduleService.GetScheduleAsync(request);

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public async Task ScheduleService_GetScheduleListAsync_ReturnsAListOfScheduleCandidate()
        {
            var userProfile = new UserProfile()
            {
                UserProfileId = 1,
                Profile = new Profile()
                {
                    Description = "Candidate"
                },
                UserId = 1
            };
            
            _mockUserProfileRepository.Setup(m => m.GetUserProfile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(userProfile));
            IList<ScheduleResponse> schedules = new List<ScheduleResponse>()
            {
                new ScheduleResponse()
                {
                    Name = "Test",
                    Profile = "Candidate"
                }
            };

            _mockScheduleRepository.Setup(s => s.GetScheduleListAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<ProfileEnum>()))
                .Returns(Task.FromResult(schedules));

            var request = new ScheduleRequest()
            {
                Name = "Test",
                Email = "email@test.com",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(1)
            };

            var scheduleService = new ScheduleService(_mockIDatabase.Object, _mockUserProfileRepository.Object, _mockScheduleRepository.Object);
            var expected = await scheduleService.GetScheduleListAsync("Test", "Candidate", DateTime.Now, DateTime.Now);

            Assert.IsNotNull(expected);
        }

        [TestMethod]
        public async Task ScheduleService_GetScheduleListAsync_ReturnsAListOfScheduleInterviewer()
        {
            var userProfile = new UserProfile()
            {
                UserProfileId = 1,
                Profile = new Profile()
                {
                    Description = "Interviewer"
                },
                UserId = 1
            };

            _mockUserProfileRepository.Setup(m => m.GetUserProfile(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(userProfile));
            IList<ScheduleResponse> schedules = new List<ScheduleResponse>()
            {
                new ScheduleResponse()
                {
                    Name = "Test",
                    Profile = "Interviewer"                
                }
            };

            _mockScheduleRepository.Setup(s => s.GetScheduleListAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<ProfileEnum>()))
                .Returns(Task.FromResult(schedules));

            var request = new ScheduleRequest()
            {
                Name = "Test",
                Email = "email@test.com",
                StartAt = DateTime.Now,
                EndAt = DateTime.Now.AddDays(1)
            };

            var scheduleService = new ScheduleService(_mockIDatabase.Object, _mockUserProfileRepository.Object, _mockScheduleRepository.Object);
            var expected = await scheduleService.GetScheduleListAsync("Test", "Interviewer", DateTime.Now, DateTime.Now);

            Assert.IsNotNull(expected);
        }
    }
}
