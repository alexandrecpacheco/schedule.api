using Dapper;
using Schedule.Domain.Dto.Response;
using Schedule.Domain.Entities;
using Schedule.Domain.Enums;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.Infrastructure.Data
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IDatabase _database;
        public ScheduleRepository(IDatabase database)
        {
            _database = database;
        }
       
        public async Task<int> Create(Domain.Entities.Schedule schedule, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            const string query = @"
                    INSERT INTO schedule (user_profile_id, [description], start_at, end_at)
                    VALUES (@UserProfileId, @Description, @StartAt, @EndAt)
            
                    SELECT @@IDENTITY;
            ";

            return await dbConnection.QuerySingleAsync<int>(query, schedule, dbTransaction);
        }

        public async Task<Domain.Entities.Schedule> GetScheduleAsync(DateTime startAt, DateTime endAt, int userProfileId)
        {
            await using var conn = await _database.CreateAndOpenConnection();

            const string query = @"
                    SELECT s.start_at, s.end_at,
                        u.user_id, u.name, u.email, 
                        up.user_profile_id,
                        p.profile_id, p.[description]
                    FROM [user] u 
                        INNER JOIN user_profile up ON up.user_id = u.user_id
						INNER JOIN schedule s ON s.user_profile_id = up.user_profile_id
                        INNER JOIN profile p ON p.profile_id = up.profile_id
                    WHERE s.start_at = @StartAt
                        AND s.end_at = @EndAt
                        AND up.user_profile_id = @UserProfileId
            ";

            var parameters = new { startAt, endAt, userProfileId };
            var scheduleDictionary = new Dictionary<int, Domain.Entities.Schedule>();
            var result = await conn.QueryAsync<Domain.Entities.Schedule, UserProfile, Profile, Domain.Entities.Schedule>(query,
                (schedule, userProfile, profile) =>
                {
                    if (schedule.UserProfile != null)
                    {
                        if (scheduleDictionary.TryGetValue(schedule.UserProfile.UserId, out var scheduleResponse) == false)
                        {
                            scheduleResponse = schedule;
                            scheduleDictionary.Add(scheduleResponse.UserProfile.UserId, scheduleResponse);
                        }

                        if (userProfile != null)
                        {
                            scheduleResponse.UserProfileId = userProfile.UserProfileId;
                            scheduleResponse.UserProfile.Profile.ProfileId = profile.ProfileId;
                            scheduleResponse.UserProfile.Profile.Description = profile.Description;
                        }

                        return scheduleResponse;
                    }
                    return new Domain.Entities.Schedule();
                }, parameters, splitOn: "user_id, user_profile_id, profile_id");

            return result.FirstOrDefault();
        }

        public async Task<IList<ScheduleResponse>> GetScheduleListAsync(DateTime startAt, DateTime endAt, ProfileEnum profile)
        {
            await using var conn = await _database.CreateAndOpenConnection();

            const string query = @"
                    SELECT s.start_at, s.end_at,
                        u.user_id, u.name, u.email, 
                        p.profile_id, p.[description]
                    FROM [user] u 
                        INNER JOIN user_profile up ON up.user_id = u.user_id
						INNER JOIN schedule s ON s.user_profile_id = up.user_profile_id
                        INNER JOIN profile p ON p.profile_id = up.profile_id
                    WHERE p.profile_id = @ProfileId
                        AND s.start_at >= Convert(datetime, @StartAt, 120)
                        AND s.end_at <= Convert(datetime, @EndAt, 120)

            ";

            var parameters = new { startAt, endAt, profileId = (int)profile };
            var result = await conn.QueryAsync<ScheduleResponse, User, Profile, ScheduleResponse>(query,
                (schedule, user, profile) =>
                {
                    var scheduleDictionary = new Dictionary<int, ScheduleResponse>();
                    if (schedule != null)
                    {
                        if (scheduleDictionary.TryGetValue(schedule.UserId, out var scheduleResponse) == false)
                        {
                            scheduleResponse = schedule;
                            scheduleDictionary.Add(scheduleResponse.UserId, scheduleResponse);
                        }

                        if (user != null)
                        {
                            scheduleResponse.Name = user.Name;
                            scheduleResponse.UserId = user.UserId;
                        }
                        if (profile != null)
                        {
                            scheduleResponse.Profile = profile.Description;
                        }

                        return scheduleResponse;
                    }
                    return new ScheduleResponse();
                }, parameters, splitOn: "user_id, profile_id");

            return result.ToList();
        }
    }
}
