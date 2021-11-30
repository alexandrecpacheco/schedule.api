using Schedule.Domain.Entities;
using System.Data.Common;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAndPassword(User userEntity);
        Task<User> GetByEmail(string email);
        Task<int> Create(User user, DbConnection dbConnection, DbTransaction dbTransaction);
    }
}
