using Microsoft.AspNetCore.Components.Server.Circuits;
using BlazorGrid.Model;
namespace BlazorGrid.Handler;

public class CInfoCircuitHandler : CircuitHandler
{
    private readonly ClientInfoState _state;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CInfoCircuitHandler(ClientInfoState state,
                             IHttpContextAccessor httpContextAccessor)
    {
        _state = state;
        _httpContextAccessor = httpContextAccessor;
    }

    public override Task OnCircuitOpenedAsync(Circuit circuit,
                                              CancellationToken ct)
    {
        var context = _httpContextAccessor.HttpContext;

        var forwardedFor = context?.Request
                                   .Headers["X-Forwarded-For"]
                                   .FirstOrDefault();

        _state.IpAddress = !string.IsNullOrEmpty(forwardedFor)
            ? forwardedFor.Split(',')[0].Trim()
            : context?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        _state.UserAgent = context?.Request
                                   .Headers["User-Agent"]
                                   .FirstOrDefault() ?? "Unknown";

        return Task.CompletedTask;
    }
}