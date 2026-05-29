using Supabase;
using Supabase.Gotrue;
using Microsoft.JSInterop;
using System.Text.Json;

namespace JobTracker.Services;

public class AuthService
{
    private readonly Supabase.Client _supabaseClient;
    private readonly IJSRuntime _jsRuntime;
    private const string SESSION_KEY = "jobtracker_session";
    private bool _sessionInitialized = false;
    private TaskCompletionSource<bool> _sessionReadyTcs = new();

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
                // After signup, sign in to establish the authenticated session
                var signInSuccess = await SignInAsync(email, password);
                return signInSuccess;
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
                // Store session to localStorage
                await SaveSessionToLocalStorageAsync(session);
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
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", SESSION_KEY);
    }

    public async Task InitializeSessionAsync()
    {
        try
        {
            // First, load any existing session from the client's persistence
            _supabaseClient.Auth.LoadSession();

            // Check if session was loaded
            if (_supabaseClient.Auth.CurrentUser != null)
            {
                _sessionInitialized = true;
                _sessionReadyTcs.SetResult(true);
                return;
            }

            // If not, try to restore session from our localStorage
            var sessionJson = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", SESSION_KEY);

            if (!string.IsNullOrEmpty(sessionJson))
            {
                try
                {
                    var session = JsonSerializer.Deserialize<Session>(sessionJson);
                    if (session != null && !string.IsNullOrEmpty(session.AccessToken) && !string.IsNullOrEmpty(session.RefreshToken))
                    {
                        // Restore session using SetSession with the stored tokens
                        var restoredSession = await _supabaseClient.Auth.SetSession(session.AccessToken, session.RefreshToken);

                        if (restoredSession?.User != null)
                        {
                            // Add a small delay to ensure the client processes the session
                            await Task.Delay(100);

                            // Check if CurrentUser is now set
                            var currentUser = _supabaseClient.Auth.CurrentUser;
                            if (currentUser != null)
                            {
                                _sessionInitialized = true;
                                _sessionReadyTcs.SetResult(true);
                            }
                            else
                            {
                                _sessionInitialized = true;
                                _sessionReadyTcs.SetResult(false);
                            }
                            return;
                        }
                    }
                }
                catch
                {
                    // Session restoration failed, continue with unauthenticated state
                }
            }

            _sessionInitialized = true;
            _sessionReadyTcs.SetResult(false);
        }
        catch
        {
            _sessionInitialized = true;
            _sessionReadyTcs.SetResult(false);
        }
    }

    public async Task WaitForSessionAsync()
    {
        if (!_sessionInitialized)
        {
            await _sessionReadyTcs.Task;
        }
    }

    private async Task SaveSessionToLocalStorageAsync(Session session)
    {
        try
        {
            var sessionJson = JsonSerializer.Serialize(session);
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", SESSION_KEY, sessionJson);
        }
        catch
        {
            // Failed to save session to localStorage
        }
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
