using Schedule.Domain.Dto.Request;
using Schedule.Domain.Entities;
using System.Threading.Tasks;

namespace Schedule.Domain.Interfaces.Data.Service
{
    public interface IUserService
    {
        Task<User> Authentication(SignInRequest signInRequest);
        Task Create(UserRequest userRequest);
    }
}
