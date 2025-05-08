using System.Security.Claims;
using System.Text.Json;
using Application.Common;
using Application.Interfaces;
using Application.Services;
using Core.Entities;
using Core.Enums;
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

            var adminLogin = Environment.GetEnvironmentVariable("ADMIN_LOGIN") ?? AppConstants.DefaultAdminLogin;
            var adminPassHash = Environment.GetEnvironmentVariable("ADMIN_PASSHASH");

            if (!string.IsNullOrEmpty(adminPassHash) && !await db.Users.AnyAsync(u => u.Login == adminLogin))
            {
                var admin = new User
                {
                    Login = adminLogin,
                    Firstname = AppConstants.DefaultAdminFirstname,
                    Lastname = AppConstants.DefaultAdminLastname,
                    Email = AppConstants.DefaultAdminEmail,
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
            // services.AddScoped<IScribanRepository, ScribanRepository>();
            services.AddScoped<AuthService>();
            services.AddScoped<SmartContractService>();

            return services;
        }

        private static IServiceCollection AddExtraAuthOptions(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "AppAuth";
                    options.Cookie.SameSite = SameSiteMode.None;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.HttpOnly = true;
                    options.LoginPath = "/login";
                    options.AccessDeniedPath = "/access-denied";

                    options.Events = new CookieAuthenticationEvents
                    {
                        OnRedirectToLogin = context =>
                        {
                            if (context.Request.Path.StartsWithSegments("/api"))
                            {
                                // context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                // return Task.CompletedTask;
                            }

                            context.Response.Redirect(context.RedirectUri);

                            return Task.CompletedTask;
                        }
                    };
                })
                .AddOAuth("GitHub", options =>
                {
                    options.ClientId = configuration["GitHubClientId"] ?? throw new ArgumentException("Not found GitHubClientId in configs");
                    options.ClientSecret = configuration["GitHubClientSecret"] ?? throw new ArgumentException("Not found GitHubClientSecret in configs");
                    options.CallbackPath = new PathString("/oauth-github");
                    options.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                    options.TokenEndpoint = "https://github.com/login/oauth/access_token";
                    options.UserInformationEndpoint = "https://api.github.com/user";

                    options.CorrelationCookie.Name = ".AspNetCore.Correlation.GitHub";
                    options.CorrelationCookie.SameSite = SameSiteMode.None;
                    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.CorrelationCookie.Path = options.CallbackPath;

                    options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                    // options.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");
                    options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email", "email");

                    options.ClaimActions.MapJsonKey("urn:github:login", "login");

                    options.Events = new OAuthEvents
                    {
                        OnCreatingTicket = async context =>
                        {
                            var http = context.HttpContext;

                            if (http is null) return;

                            var userInfoEndpoint = context.Options.UserInformationEndpoint;
                            var request = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);
                            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", context.AccessToken);

                            var response = await context.Backchannel.SendAsync(request);
                            response.EnsureSuccessStatusCode();

                            var payload = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

                            context.RunClaimActions(payload.RootElement);

                            var login = context.Properties.Items.TryGetValue("login", out var loginValue)
                                ? loginValue
                                : context?.Principal?.Identity?.Name;

                            var githubLogin = context?.Principal?.FindFirst("urn:github:login")?.Value;

                            Console.WriteLine($"GitHub login: {githubLogin}, Local login: {login}");

                            if (login == null || githubLogin == null)
                            {
                                return;
                            }

                            var authService = context?.HttpContext.RequestServices.GetRequiredService<AuthService>();

                            if (authService == null)
                            {
                                return;
                            }

                            try
                            {
                                await authService.LinkGitHub(login, githubLogin);

                                var result = await authService.GetUserByLogin(login);

                                if (!result.Succeeded || result.Payload is null) return;

                                var userDto = result.Payload;
                                var id = (ClaimsIdentity)context?.Principal!.Identity!;
                                var oldName = id.FindFirst(ClaimTypes.Name);

                                if (oldName is not null) id.RemoveClaim(oldName);

                                id.AddClaim(new Claim(ClaimTypes.Role, userDto.Role.ToLowerInvariant()));
                                id.AddClaim(new Claim(ClaimTypes.Name, login));
                                id.AddClaim(new Claim("urn:github:login", githubLogin));

                                if (context is null || context.Principal is null)
                                {
                                    return;
                                }

                                await context.HttpContext.SignInAsync(
                                    CookieAuthenticationDefaults.AuthenticationScheme,
                                    context.Principal,
                                    new AuthenticationProperties
                                    {
                                        IsPersistent = true,
                                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7),
                                        AllowRefresh = true
                                    });
                            }
                            catch (Exception ex)
                            {
                                var message = ex.Message;

                                // context.Fail("Account is already linked to another user");
                                return;
                            }
                        },
                        OnRemoteFailure = context =>
                        {
                            context.Response.Cookies.Delete(".AspNetCore.Correlation.GitHub");

                            var error = Uri.EscapeDataString("GitHub authorization was denied.");

                            Console.WriteLine($"OAuth Error: {context.Failure?.Message}");
                            Console.WriteLine($"Error: {error}");

                            context.Response.Redirect($"/login?errorMessage={error}");
                            context.HandleResponse();

                            return Task.CompletedTask;
                        }
                    };
                });

            return services;
        }
    }
}
