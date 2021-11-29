using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Schedule.Api.Service;
using Schedule.Domain;
using Schedule.Domain.Interfaces.Data;
using Schedule.Domain.Interfaces.Data.Repository;
using Schedule.Domain.Interfaces.Data.Service;
using Schedule.Infrastructure;
using Schedule.Infrastructure.Data;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Schedule.Api
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            HostEnvironment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile($"appsettings.{HostEnvironment.EnvironmentName}.json", true);

            if (builder != null) Configuration = builder.Build();
            Log.Information("ENVIRONMENT: {CurrentEnvironment}", HostEnvironment.EnvironmentName);
        }
        public IConfigurationRoot Configuration { get; }
        private IHostEnvironment HostEnvironment { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomSwagger();
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            services.AddRazorPages();

            services.AddSingleton<IDatabase, Database>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            var apiSettings = new ApiSettings();
            Configuration.Bind("Schedule", apiSettings);
            apiSettings.Environment.CurrentEnvironment = HostEnvironment.EnvironmentName;

            services.AddCustomCors();
            services.AddCustomAuthorization();
            services.AddSingleton(apiSettings);
            services.AddCustomAuthentication(apiSettings);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseCustomSwagger(env.EnvironmentName);
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
