using Microsoft.AspNetCore.DataProtection;

namespace BlazorGrid.Services;

public class QueryParamProtector
{
    private readonly ITimeLimitedDataProtector  _protector;

    public QueryParamProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("AuthFlow.QueryParams").ToTimeLimitedDataProtector();
    }

    public string Protect(string username, string redirectUrl)
    {
        var payload = $"{username}|{redirectUrl}";
        return _protector.Protect(payload, TimeSpan.FromSeconds(20));
    }

    public bool TryUnprotect(string? token, out string username, out string redirectUrl)
    {
        username = string.Empty;
        redirectUrl = "/";

        if (string.IsNullOrEmpty(token))
            return false;

        try
        {
            var payload = _protector.Unprotect(token);
            var parts = payload.Split('|', 2);
            username = parts[0];
            redirectUrl = parts.Length > 1 ? parts[1] : "/";
            return true;
        }
        catch
        {
            return false;
        }
    }
}
