
using System.Threading.RateLimiting;
using BlazorGrid.Components;
using BlazorGrid.Services;
using BlazorGrid.Model;
using BlazorGrid.Handler;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Components.Server.Circuits;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(1);
    });
builder.Services.AddSingleton<QueryParamProtector>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 6000,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));

    options.RejectionStatusCode = 429;
});

builder.Services.AddHttpContextAccessor(); 
builder.Services.AddScoped<ClientInfoState>();
builder.Services.AddScoped<CircuitHandler, CInfoCircuitHandler>();
var app = builder.Build();

if (!app.Environment.IsDevelopment())

{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRateLimiter();
app.UseAntiforgery();

app.MapHealthChecks("/health");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
