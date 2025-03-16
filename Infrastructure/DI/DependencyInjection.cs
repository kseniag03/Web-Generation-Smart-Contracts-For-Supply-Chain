using Application.Services;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DI
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            string rpcUrl = "",
            string connectionString = "")
        {
            // services.AddSingleton<IBlockchainService>(new NethereumBlockchainService(rpcUrl));

            services.AddDbContext<ContractsDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = configuration["GitHubClientId"];
                    options.ClientSecret = configuration["GitHubClientSecret"];
                    options.CallbackPath = new PathString("/signin-github");
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";
                    options.Events = new OAuthEvents
                    {
                        OnRemoteFailure = context =>
                        {
                            Console.WriteLine($"OAuth Error: {context.Failure?.Message}");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ISmartContractRepository, SmartContractRepository>();
            services.AddScoped<UserService>();
            services.AddScoped<AuthService>();
            services.AddScoped<SmartContractService>();

            return services;
        }
    }
}
