using Infrastructure.Data;
using Infrastructure.DI;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Net;
using System.Runtime.InteropServices;
using Utilities.DI;
using WebApp.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents(options =>
    options.DetailedErrors = builder.Environment.IsDevelopment())
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

var cookieJar = new CookieContainer();
builder.Services.AddSingleton(cookieJar);

var apiBase = builder.Configuration["API_BASE_URL"] ?? throw new ArgumentException("Not found API_BASE_URL in configs");

builder.Services.AddHttpClient("with-cookies", (sp, client) =>
{
    client.BaseAddress = new Uri(apiBase);
    client.Timeout = TimeSpan.FromMinutes(10);
})
.ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
{
    UseCookies = true,
    CookieContainer = sp.GetRequiredService<CookieContainer>(),
    AllowAutoRedirect = false     // very important
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddUtilities();

if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    builder.Services.AddDataProtection()
        .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"));
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();

if (!app.Environment.IsDevelopment())
{
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedProto,
        KnownProxies = { IPAddress.Parse("127.0.0.1") }
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContractsDbContext>();
    var pendingMigrations = db.Database.GetPendingMigrations().ToList();

    Console.WriteLine("Pending migrations:");
    pendingMigrations.ForEach(Console.WriteLine);

    try
    {
        db.Database.Migrate();
        await Infrastructure.DI.DependencyInjection.SeedAsync(app.Services);
    }
    catch (PostgresException ex) when (ex.SqlState == "42P07")
    {
        var message = ex.Message;
    }
    catch (Exception ex)
    {
        var message = ex.Message;
    }
}

app.Logger.LogInformation("App has started in {env}", app.Environment.EnvironmentName);

app.Run();
