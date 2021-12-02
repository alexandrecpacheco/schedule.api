using Schedule.Domain.Dto.Request;
using Schedule.Domain.Dto.Response;
using Schedule.Domain.Enums;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Schedule.Service
{
    public class ScheduleService : IScheduleService
    {
        private readonly IDatabase _database;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IScheduleRepository _scheduleRepository;
        public ScheduleService(IDatabase database,
            IUserProfileRepository userProfileRepository,
            IScheduleRepository scheduleRepository)
        {
            _database = database;
            _userProfileRepository = userProfileRepository;
            _scheduleRepository = scheduleRepository;
        }

        public async Task Create(ScheduleRequest request)
        {
            var validateHours = Domain.Utils.Validator.ValidateClosedHours(request.StartAt, request.EndAt);
            if (validateHours == false)
                throw new ArgumentException(@$"The time {request.StartAt} and {request.EndAt} 
                    is not permitted, please try again with a valid time");

            var validDates = Domain.Utils.Validator.ValidateDates(request.StartAt, request.EndAt);
            if (validDates == false)
                throw new ArgumentException(@$"The period {request.StartAt} and {request.EndAt} 
                    is not between a valid date, please try again with a valid period");

            var scheduleAlreadyUsed = await GetScheduleAsync(request);
            if (scheduleAlreadyUsed != null)
            {
                Log.Information("Schedule already exists, try again with another period");
                throw new DuplicateNameException($"Schedule already exists, try again with another period");
            }

            var userProf = await _userProfileRepository.GetUserProfile(request.Name, request.Email);
            if (userProf == null)
            {
                Log.Error($"The UserProfile {request.Name} does not exists");
                throw new KeyNotFoundException($"The UserProfile {request.Name} does not exists");
            }

            await _database.ExecuteInTransaction(async (connection, transaction) =>
            {
                var schedule = new Domain.Entities.Schedule()
                {
                    Description = request.Description,
                    StartAt = request.StartAt,
                    EndAt = request.EndAt,
                    UserProfileId = userProf.UserProfileId
                };

                Log.Information("Insert a new schedule");
                var scheduleRepo = await _scheduleRepository.Create(schedule, connection, transaction);
            });
        }

        public async Task<Domain.Entities.Schedule> GetScheduleAsync(ScheduleRequest request)
        {
            var userProfile = await _userProfileRepository.GetUserProfile(request.Name, request.Email);
            if (userProfile == null)
            {
                Log.Error($"The UserProfile {request.Name} does not exists");
                throw new ArgumentNullException($"The UserProfile {request.Name} does not exists");
            }

            return await _scheduleRepository.GetScheduleAsync(request.StartAt, request.EndAt, userProfile.UserProfileId);
        }

        public async Task<IList<ScheduleResponse>> GetScheduleListAsync(string name, string profile, DateTime startAt, DateTime endAt)
        {
            var userProfile = await _userProfileRepository.GetUserProfile(name, string.Empty);
            if (userProfile == null)
            {
                Log.Warning($"The UserProfile {name} does not exists");
                throw new ArgumentNullException("The informed name does not exists, please inform a valid name");
            }

            if (Enum.TryParse(profile, out ProfileEnum profileResult))
            {
                IList<ScheduleResponse> scheduleResults = new List<ScheduleResponse>();
                if (profileResult == ProfileEnum.Candidate)
                    scheduleResults = await _scheduleRepository.GetScheduleListAsync(startAt, endAt, ProfileEnum.Interviewer);
                else
                    scheduleResults = await _scheduleRepository.GetScheduleListAsync(startAt, endAt, ProfileEnum.Candidate);

                SlotTime(scheduleResults);

                return scheduleResults;
            }

            return new List<ScheduleResponse>();
        }

        private void SlotTime(IList<ScheduleResponse> responses)
        {
            foreach (var item in responses)
            {
                item.SlotTime.Add(item.StartAt);
                DateTime startTime = item.StartAt;
                DateTime endTime = item.EndAt;
                while (startTime != endTime)
                {
                    double minuts = +60;
                    startTime = startTime.AddMinutes(minuts);
                    item.SlotTime.Add(startTime);
                }
            }
        }
    }
}
