using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ApplicationCore.Model;
using IdentityProvider.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityProvider
{
    public static class JwtSecurityService
    {
        public static void RegisterTokenAuthentication(IServiceCollection services, IConfiguration configuration)
        {
            // ===== Add Identity ========
            var builder = services.AddIdentityCore<User>();
            builder = new IdentityBuilder(builder.UserType, typeof(Role), builder.Services);
            builder
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>()
                .AddRoleManager<RoleManager<Role>>()
                .AddUserManager<UserManager<User>>();

            // ===== Add Jwt Authentication ========
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["Tokens:JwtIssuer"],
                        ValidAudience = configuration["Tokens:JwtIssuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Tokens:JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });
        }
    }
}