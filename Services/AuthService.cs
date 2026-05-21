using Supabase;
using Supabase.Gotrue;

namespace JobTracker.Services;

public class AuthService
{
    private readonly Supabase.Client _supabaseClient;

    public AuthService(Supabase.Client supabaseClient)
    {
        _supabaseClient = supabaseClient;
    }

    public async Task<bool> SignUpAsync(string email, string password)
    {
        try
        {
            var session = await _supabaseClient.Auth.SignUp(email, password);
            return session?.User != null;
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
            return session?.User != null;
        }
        catch
        {
            return false;
        }
    }

    public async Task SignOutAsync()
    {
        await _supabaseClient.Auth.SignOut();
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
