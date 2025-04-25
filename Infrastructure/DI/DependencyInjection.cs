using System.Text.Json;
using Application.Services;
using Core.Entities;
using Core.Enums;
using Core.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Helpers;
using Microsoft.AspNetCore.Authentication;
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
        public static async Task SeedAsync(IServiceProvider sp)
        {
            await using var scope = sp.CreateAsyncScope();

            var db = scope.ServiceProvider.GetRequiredService<ContractsDbContext>();

            foreach (var r in Enum.GetValues<RoleType>())
            {
                var name = r.ToString();

                if (!await db.Roles.AnyAsync(x => x.RoleName == name))
                {
                    db.Roles.Add(new Role { RoleName = name });
                }
            }

            var adminLogin = Environment.GetEnvironmentVariable("ADMIN_LOGIN") ?? "admin";
            var adminPassHash = Environment.GetEnvironmentVariable("ADMIN_PASSHASH");

            if (!string.IsNullOrEmpty(adminPassHash) && !await db.Users.AnyAsync(u => u.Login == adminLogin))
            {
                var admin = new User
                {
                    Login = adminLogin,
                    Firstname = "System",
                    Lastname = "Administrator",
                    Email = "admin@example.com",
                    GitHubId = string.Empty
                };

                admin.Userauth = new Userauth
                {
                    IdUser = admin.IdUser,
                    PasswordHash = adminPassHash
                };

                await RoleHelper.AssignRole(db, admin, RoleType.Admin);

                db.Users.Add(admin);
            }

            await db.SaveChangesAsync();
        }

        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration,
            string rpcUrl = "",
            string connectionString = "")
        {
            // services.AddSingleton<IBlockchainService>(new NethereumBlockchainService(rpcUrl));

            services.AddDbContext<ContractsDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddExtraAuthOptions(configuration);

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<ISmartContractRepository, SmartContractRepository>();
            services.AddScoped<AuthService>();
            services.AddScoped<SmartContractService>();

            return services;
        }

        private static void AddExtraAuthOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AppAuth";
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.None;
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/access-denied";

                    // переопределяем поведение для API‑маршрутов
                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = ctx =>
                        {
                            if (ctx.Request.Path.StartsWithSegments("/api"))
                            {
                                // для /api/** – отдать 401, а не 302
                                ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                return Task.CompletedTask;
                            }

                            // для обычных страниц сохраняем редирект
                            ctx.Response.Redirect(ctx.RedirectUri);
                            return Task.CompletedTask;
                        }
                    };
                })
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = configuration["GitHubClientId"];
                    options.ClientSecret = configuration["GitHubClientSecret"];
                    options.CallbackPath = new PathString("/signin-github");
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.ClaimActions.MapJsonKey("login", "login");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var userInfoEndpoint = context.Options.UserInformationEndpoint;
                            var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                            var githubLogin = payload.RootElement.GetProperty("login").GetString();

                            // Получаем текущего пользователя
                            var userName = context?.Principal?.Identity?.Name;

                            // Получаем нужный сервис из контейнера
                            var authService = context?.HttpContext.RequestServices.GetRequiredService<AuthService>();

                            if (userName == null || githubLogin == null || authService == null)
                            {
                                return;
                            }

                            await authService.LinkGitHub(userName, githubLogin);
                        },
                        OnRemoteFailure = context =>
                        {
                            // Перенаправляем обратно на /login с сообщением об ошибке
                            var error = Uri.EscapeDataString("GitHub authorization was denied.");

                            Console.WriteLine($"OAuth Error: {context.Failure?.Message}");
                            Console.WriteLine($"Error: {error}");

                            // Перенаправляем на страницу с ошибкой
                            context.Response.Redirect($"/login?errorMessage={error}");
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });


        }
    }
}
