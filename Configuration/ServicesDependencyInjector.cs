using ApplicationCore.Interfaces;
using IdentityProvider;
using Infrastructure.Mapping;
using Infrastructure.Repository;
using LdapProvider;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration
{
    public static class ServicesDependencyInjector
    {
        public static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IUnityOfWork, UnitOfWork>();
            services.AddSingleton<IMappingCache, MappingCache>();
            services.AddTransient<ITokenGenerator, JwtTokenGenerator>();
            services.AddTransient<IAuthenticationService, LdapAuthenticationService>();
            JwtSecurityService.RegisterTokenAuthentication(services, configuration);
        }
    }
}