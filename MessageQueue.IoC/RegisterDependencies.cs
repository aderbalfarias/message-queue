using MessageQueue.Data.Config;
using MessageQueue.Data.Repositories;
using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Domain.Interfaces.Services;
using MessageQueue.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MessageQueue.IoC
{
    public static class RegisterDependencies
    {
        public static IServiceCollection Classes(this IServiceCollection services, IConfiguration configurantionSession)
        {
            //services.AddSingleton<AppSettings>(configurantionSession.GetSection(typeof(AppSettings).Name ));

            return services;
        }

        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddTransient<ICommandService, CommandService>();
            services.AddTransient<IEventService, EventService>();

            return services;
        }

        public static IServiceCollection Repositories(this IServiceCollection services)
        {
            services.AddTransient<IBaseRepository, BaseRepository>();

            return services;
        }

        public static IServiceCollection Databases(this IServiceCollection services, string connectionString)
        {
            // Add configuration for DbContext
            // Use connection string from appsettings.json file
            services.AddDbContext<NserviceBusContext>(options =>
                options.UseSqlServer(connectionString), ServiceLifetime.Transient, ServiceLifetime.Transient);

            return services;
        }
    }
}
