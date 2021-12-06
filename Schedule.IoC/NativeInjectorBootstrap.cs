using Schedule.Api.Service;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using Schedule.Infrastructure;
using Schedule.Infrastructure.Data;
using Schedule.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Schedule.IoC
{
    public static class NativeInjectorBootstrap
    {
        public static void RegisterDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IDatabase, Database>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IScheduleService, ScheduleService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserProfileRepository, UserProfileRepository>();
            services.AddScoped<IScheduleRepository, ScheduleRepository>();
        }
    }
}
