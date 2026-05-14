
using System.Threading.RateLimiting;
using BlazorGrid.Components;
using BlazorGrid.Services;
using Microsoft.AspNetCore.RateLimiting;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR()
    .AddMessagePackProtocol();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(1);
    })
    .AddHubOptions(options =>
    {
        options.SupportedProtocols = null;
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
