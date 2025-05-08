using WebApp.Components;
using Utilities.DI;
using Infrastructure.DI;
using Microsoft.AspNetCore.DataProtection;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Npgsql;
using Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

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


builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ContractsDbContext>();

    try
    {
        db.Database.Migrate();
        await Infrastructure.DI.DependencyInjection.SeedAsync(app.Services);
    }
    catch (PostgresException ex) when (ex.SqlState == "42P07")
    {
        // Table already exists, continue silently
        var message = ex.Message;
    }
    catch (Exception ex)
    {
        // another problem
        var message = ex.Message;
    }
}

// !!!!!!!!!! debug scriban

try
{
    var repo = new ScribanRepository(builder.Configuration);

    repo.Init();
    repo.Init("IoT");
    repo.Init("Pharmaceutics");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// !!!!!!!!!! debug scriban

app.Run();
