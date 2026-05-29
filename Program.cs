using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Supabase;
using JobTracker;
using JobTracker.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Supabase client configuration
var supabaseUrl = builder.Configuration["Supabase:Url"] ?? throw new InvalidOperationException("Supabase:Url not configured");
var supabaseKey = builder.Configuration["Supabase:Key"] ?? throw new InvalidOperationException("Supabase:Key not configured");

var options = new SupabaseOptions
{
    AutoConnectRealtime = true,
    AutoRefreshToken = true
};

builder.Services.AddScoped(_ => new Supabase.Client(supabaseUrl, supabaseKey, options));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JobService>();

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
