using WebApp.Components;
using Utilities.DI;
using Infrastructure.DI;
using Microsoft.AspNetCore.DataProtection;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var cookieJar = new CookieContainer();
builder.Services.AddSingleton(cookieJar);

var apiBase = builder.Configuration["API_BASE_URL"] ?? "http://localhost:8080";

builder.Services.AddHttpClient("with-cookies", (sp, client) =>
{
    client.BaseAddress = new Uri(apiBase);
})
.ConfigurePrimaryHttpMessageHandler(sp => new HttpClientHandler
{
    UseCookies = true,
    CookieContainer = sp.GetRequiredService<CookieContainer>()
});

builder.Services.AddInfrastructure(builder.Configuration);
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
    db.Database.Migrate();
    await Infrastructure.DI.DependencyInjection.SeedAsync(app.Services);
}

app.Run();
