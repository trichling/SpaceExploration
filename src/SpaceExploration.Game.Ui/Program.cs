using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.Game.Ui;
using SpaceExploration.Game.Ui.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<GameStateService>();  

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Ui");
endpointConfiguration.UsePersistence<LearningPersistence>();

var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("..\\transport");

var routing = transport.Routing();  
routing.RouteToEndpoint(typeof(CreatePlanet).Assembly, "SpaceExploration.Game");

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

builder.UseNServiceBus(endpointConfiguration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
