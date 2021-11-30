using Schedule.Domain.Entities;
using System.Data.Common;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Repository
{
    public interface IUserProfileRepository
    {
        Task<int> Create(UserProfile userProfile, DbConnection dbConnection, DbTransaction dbTransaction);
    }
}
