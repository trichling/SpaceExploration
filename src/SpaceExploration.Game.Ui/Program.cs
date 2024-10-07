using Microsoft.Data.SqlClient;

using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Game.Contracts.Planets.Commands;
using SpaceExploration.Game.Ui;
using SpaceExploration.Game.Ui.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<GameStateService>();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Ui");
// var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
// persistence.SagaStorageDirectory("..\\sagas");
//var persistence = endpointConfiguration.UsePersistence<NonDurablePersistence>();
var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.SqlDialect<SqlDialect.MsSqlServer>();
persistence.ConnectionBuilder(
    connectionBuilder: () =>
    {
        return new SqlConnection(builder.Configuration["ConnectionStrings:Persistence"]);
    });

// var transport = endpointConfiguration.UseTransport<LearningTransport>();
// transport.StorageDirectory("..\\transport");

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.ConnectionString(builder.Configuration["ConnectionStrings:AzureServiceBus"]);
transport.SubscriptionRuleNamingConvention(type => type.Name);

var routing = transport.Routing();
routing.RouteToEndpoint(typeof(CreatePlanet).Assembly, "SpaceExploration.Game");
routing.RouteToEndpoint(typeof(Move).Assembly, "SpaceExploration.Game");

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.EnableInstallers();

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
