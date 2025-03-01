using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Executors;
using Utilities.Interfaces;

namespace Utilities.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddUtilities(this IServiceCollection services)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                services.AddSingleton<ICommandExecutor, WindowsCommandExecutor>();
            else
                services.AddSingleton<ICommandExecutor, UnixCommandExecutor>();

            /*
            services.AddScoped<IHardhatExecutor, HardhatExecutor>();
            services.AddScoped<IFoundryExecutor, FoundryExecutor>();
            services.AddScoped<ISlitherExecutor, SlitherExecutor>();
            services.AddScoped<IMythrilExecutor, MythrilExecutor>();
            */

            return services;
        }
    }

}
