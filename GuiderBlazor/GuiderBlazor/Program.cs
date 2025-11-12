using GuiderBlazor.Components;
using GuiderBlazor.Shared.Services;
//using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp => new HttpClient
{
    //BaseAddress = new Uri("https://api.guider.pro/")
    BaseAddress = new Uri("https://localhost:8081/")

});
builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.AddScoped<ICitiesService, CitiesService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GuiderBlazor.Client._Imports).Assembly);

app.Run();
