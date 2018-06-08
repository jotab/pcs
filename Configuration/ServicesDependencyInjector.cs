using ApplicationCore.Interfaces;
using Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace Configuration
{
    public static class ServicesDependencyInjector
    {
        public static void RegisterServices(IServiceCollection services)
        {
            services.AddScoped<IUnityOfWork, UnitOfWork>();
        }
    }
}