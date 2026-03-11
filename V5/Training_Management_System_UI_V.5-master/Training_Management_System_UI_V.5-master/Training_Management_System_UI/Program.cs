using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Training_Management_System_UI;
using Training_Management_System_UI.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// LOGIN
builder.Services.AddHttpClient<AuthService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7088/");
});

// ADMIN DASHBOARD
builder.Services.AddHttpClient<TrainingService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7088/");
});

// SESSION - Singleton so the same instance is shared across all components
builder.Services.AddSingleton<UserSessionService>();
builder.Services.AddHttpClient<UserSessionService>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7088/");
});

await builder.Build().RunAsync();