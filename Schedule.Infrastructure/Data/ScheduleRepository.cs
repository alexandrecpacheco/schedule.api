using Dapper;
using Schedule.Domain.Entities;
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
                    FROM schedule s
                        INNER JOIN [user] u ON u.user_id = u.user_id
                        INNER JOIN user_profile up ON s.user_profile_id = up.user_profile_id
                            AND up.user_id = u.user_id
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
    }
}
