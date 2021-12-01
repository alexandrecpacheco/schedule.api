using Dapper;
using Schedule.Domain.Entities;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace Schedule.Infrastructure.Data
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly IDatabase _database;
        public UserProfileRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<int> Create(UserProfile userProfile, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            const string query = @"
                    INSERT INTO user_profile (user_id, profile_id)
                    VALUES (@UserId, @ProfileId);
            
                    SELECT @@IDENTITY;
            ";

            return await dbConnection.QuerySingleAsync<int>(query, userProfile, dbTransaction);
        }

        public async Task<UserProfile> GetUserProfile(string name, string email)
        {
            await using var conn = await _database.CreateAndOpenConnection();

            string query = @"
                    SELECT up.user_profile_id, up.user_id,
                           p.profile_id, p.description
                    FROM [user] u
                    INNER JOIN user_profile up
                        ON u.user_id = up.user_id
                    INNER JOIN profile p 
                        ON up.profile_id = p.profile_id
                    WHERE name = @Name
                    ";

            query += !string.IsNullOrWhiteSpace(email) ? " AND email = @Email" : string.Empty;

            var parameters = new { email, name };
            var userDictionary = new Dictionary<int, UserProfile>();
            var result = await conn.QueryAsync<UserProfile, Profile, UserProfile>(query,
                (userProfile, profile) =>
                {
                    if (userDictionary.TryGetValue(userProfile.UserId, out var userResponse) == false)
                    {
                        userResponse = userProfile;
                        userDictionary.Add(userResponse.UserId, userResponse);
                    }

                    if (userProfile != null)
                    {
                        userResponse.UserProfileId = userProfile.UserProfileId;
                        userResponse.Profile.ProfileId = profile.ProfileId;
                        userResponse.Profile.Description = profile.Description;

                    }
                    return userResponse;
                }, parameters, splitOn: "profile_id");

            return result.FirstOrDefault();
        }
    }
}
