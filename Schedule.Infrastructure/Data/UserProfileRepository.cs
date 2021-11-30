using Dapper;
using Schedule.Domain.Entities;
using Schedule.Domain.Interfaces.Data.Repository;
using System.Data.Common;
using System.Threading.Tasks;

namespace Schedule.Infrastructure.Data
{
    public class UserProfileRepository : IUserProfileRepository
    {
        public async Task<int> Create(UserProfile userProfile, DbConnection dbConnection, DbTransaction dbTransaction)
        {
            const string query = @"
                    INSERT INTO user_profile (user_id, profile_id)
                    VALUES (@UserId, @ProfileId);
            
                    SELECT @@IDENTITY;
            ";

            return await dbConnection.QuerySingleAsync<int>(query, userProfile, dbTransaction);
        }
    }
}
