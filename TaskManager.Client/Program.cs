using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using TaskManager.Client.Components;
using TaskManager.Client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
       
    });


builder.Services.AddHttpClient();
builder.Services.AddScoped<JwtStorageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<TaskService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
