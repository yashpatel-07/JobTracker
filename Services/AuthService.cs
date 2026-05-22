using Supabase;
using Supabase.Gotrue;
using Microsoft.JSInterop;

namespace JobTracker.Services;

public class AuthService
{
    private readonly Supabase.Client _supabaseClient;
    private readonly IJSRuntime _jsRuntime;

    public AuthService(Supabase.Client supabaseClient, IJSRuntime jsRuntime)
    {
        _supabaseClient = supabaseClient;
        _jsRuntime = jsRuntime;
    }

    public async Task<bool> SignUpAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignUp(email, password);
            if (session?.User != null)
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SignInAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignInWithPassword(email, password);
            if (session?.User != null)
            {
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        await _supabaseClient.Auth.SignOut();
        // Clear session from localStorage
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "sb-sbpersist");
    }

    public Session? GetCurrentSession()
    {
        return _supabaseClient.Auth.CurrentSession;
    }

    public User? GetCurrentUser()
    {
        return _supabaseClient.Auth.CurrentUser;
    }

    public bool IsAuthenticated => _supabaseClient.Auth.CurrentUser != null;


}
