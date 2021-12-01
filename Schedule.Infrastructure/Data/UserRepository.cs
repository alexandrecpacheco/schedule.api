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
    public class UserRepository : IUserRepository
    {
        private readonly IDatabase _database;
        public UserRepository(IDatabase database)
        {
            _database = database;
        }

        public async Task<User> GetByEmailAndPassword(User userEntity)
        {
            await using var conn = await _database.CreateAndOpenConnection();

            const string query = @"
                    SELECT u.user_id, u.user_key, u.name, u.email, u.is_active, u.created_at, u.updated_at,
                           up.user_profile_id, 
                           p.profile_id, p.description
                    FROM [user] u
                    INNER JOIN user_profile up
                        ON u.user_id = up.user_id
                    INNER JOIN profile p 
                        ON up.profile_id = p.profile_id
                    WHERE email = @Email 
                    AND password = @Password";

            var parameters = new { userEntity.Email, userEntity.Password };
            var userDictionary = new Dictionary<int, User>();
            var result = await conn.QueryAsync<User, UserProfile, Profile, User>(query,
                (user, userProfile, profile) =>
                {
                    if (userDictionary.TryGetValue(user.UserId, out var userResponse) == false)
                    {
                        userResponse = user;
                        userDictionary.Add(userResponse.UserId, userResponse);
                    }

                    if (userProfile != null)
                    {
                        userResponse.UserProfile.UserProfileId = userProfile.UserProfileId;
                        userResponse.UserProfile.Profile.ProfileId = profile.ProfileId;
                        userResponse.UserProfile.Profile.Description = profile.Description;

                    }
                    return userResponse;
                }, parameters, splitOn: "user_profile_id, profile_id");

            return result.FirstOrDefault();
        }

        public async Task<User> GetByEmail(string email)
        {
            await using var conn = await _database.CreateAndOpenConnection();
            const string query = @"
                    SELECT u.user_id, u.name, u.email
                    FROM [user] u
                    WHERE u.email = @Email
            ";

            var parameters = new { email };
            return await conn.QueryFirstOrDefaultAsync<User>(query, parameters);
        }

        public async Task<int> Create(User user, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            const string query = @"
                    INSERT INTO [user] (name, email, password, is_active)
                    VALUES (@Name, @Email, @Password, @IsActive);
                    
                    SELECT @@IDENTITY;
            ";

            return await dbConnection.QuerySingleAsync<int>(query, user, dbTransaction);
        }
    }
}
