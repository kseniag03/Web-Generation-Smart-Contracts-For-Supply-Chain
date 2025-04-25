using Microsoft.Extensions.DependencyInjection;
using Utilities.Executors;
using Utilities.Interfaces;

namespace Utilities.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            services.AddSingleton<ICommandExecutor, CommandExecutor>();
            services.AddScoped<IHardhatExecutor, HardhatExecutor>();
            services.AddScoped<IFoundryExecutor, FoundryExecutor>();
            services.AddScoped<ISlitherExecutor, SlitherExecutor>();

            return services;
        }
    }
}
