using Application.Services;
using Core.Interfaces;
using Infrastructure.Blockchain;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string rpcUrl = "", string connectionString = "")
        {
            // services.AddSingleton<IBlockchainService>(new NethereumBlockchainService(rpcUrl));

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISmartContractRepository, SmartContractRepository>();
            services.AddScoped<SmartContractService>();

            return services;
        }
    }
}
