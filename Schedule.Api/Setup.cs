using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Schedule.Domain;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Schedule
{
    public static class Setup
    {
        public static void AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Schedule",
                    Description = "Schedule Api",
                    Contact = new OpenApiContact { Name = "Schedule" }
                });
                c.AddSecurityDefinition("Bearer",
                    new OpenApiSecurityScheme
                    {
                        Description = "Please enter into field the word 'Bearer' following by space and JWT",
                        BearerFormat = "apiKey",
                        Name = "Authorization",
                        In = ParameterLocation.Header
                    });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app, string environment)
        {
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schedule");
                c.DocumentTitle = $"{"Schedule"} - {environment}";
                c.RoutePrefix = string.Empty;
            });
        }

        public static void AddCustomAuthentication(this IServiceCollection services, ApiSettings apiSettings)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = apiSettings.Values.Issuer,
                        ValidAudience = apiSettings.Values.Audience,
                        IssuerSigningKey = Create(apiSettings.Values.SecurityKey)
                    };
                    options.IncludeErrorDetails = true;
                });
        }

        public static void AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(config =>
            {
                config.AddPolicy("Admin",
                    policyBuilder => { policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"); });

                config.AddPolicy("Interviewer",
                    policyBuilder => { policyBuilder.RequireClaim(ClaimTypes.Role, "Interviewer", "Admin"); });

                config.AddPolicy("Candidate",
                    policyBuilder => { policyBuilder.RequireClaim(ClaimTypes.Role, "Candidate", "Admin"); });
            });
        }

        public static void AddCustomCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowPolicy", policy => policy
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        private static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }

    }
}