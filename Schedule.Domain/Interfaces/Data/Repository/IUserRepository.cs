using Schedule.Domain.Entities;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Repository
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAndPassword(User userEntity);
    }
}
