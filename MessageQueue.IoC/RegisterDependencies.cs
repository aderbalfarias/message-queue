using MessageQueue.Data.Config;
using MessageQueue.Data.Repositories;
using MessageQueue.Domain.Entities;
using MessageQueue.Domain.Interfaces.Repositories;
using MessageQueue.Domain.Interfaces.Services;
using MessageQueue.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MessageQueue.IoC
{
    public static class RegisterDependencies
    {
        public static IServiceCollection Classes(this IServiceCollection services, IConfigurationSection configurantionSession)
        {
            services.AddSingleton<AppSettings>(configurantionSession.Get<AppSettings>());

            return services;
        }

        public static IServiceCollection Services(this IServiceCollection services)
        {
            services.AddScoped<ICommandService, CommandService>();

            return services;
        }

        public static IServiceCollection Repositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository, BaseRepository>();

            return services;
        }

        public static IServiceCollection Databases(this IServiceCollection services, string connectionString)
        {
            // Add configuration for DbContext
            // Use connection string from appsettings.json file
            services.AddDbContext<PrimaryContext>(options => options.UseSqlServer(connectionString));

            return services;
        }
    }
}
