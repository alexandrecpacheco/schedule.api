using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Microsoft.Extensions.Configuration;

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

        public static void AddCustomAuthentication(this IServiceCollection services)
        {
            var securityKey0 = System.Environment.GetEnvironmentVariable("SecurityKey");
            //var issuer = System.Environment.GetEnvironmentVariable("Issuer");
            //var audience = System.Environment.GetEnvironmentVariable("Audience");
            var securityKey = "knD2&G|/fae0I1@iP64l{>2+jL7UNF1Tb<|P`|2.q2}Qpsr$M2Bb71FPm45F]G";
            var issuer = "http://localhost";
            var audience = "Schedule Api";

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = Create(securityKey)
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