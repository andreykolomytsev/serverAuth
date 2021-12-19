using Application.Interfaces;
using Infrastructure.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Shared
{
    public static class ServiceRegistration
    {
        /// <summary>
        /// Общая инфраструктура приложения
        /// </summary>
        /// <param name="services"></param>
        /// <param name="_config"></param>
        public static void AddSharedInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeService, DateTimeService>();
        }
    }
}
