using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using MudBlazor.Services;
using SlicaWeb.Common;
using SlicaWeb.Data;
using SlicaWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
});
// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices(
    config=>config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.TopCenter
    );
builder.Services.AddScoped<CookieManagement>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSignalRCore();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseEndpoints(endpoints => {  endpoints.MapHub<NotificationHub>("/NotificationHub"); });
app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
