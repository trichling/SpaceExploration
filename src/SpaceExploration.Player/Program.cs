using Microsoft.Data.SqlClient;
using SpaceExploration.Game.Contracts.Drones.Commands;
using SpaceExploration.Player;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHostedService<Player>();

var endpointConfiguration = ConfigureEndpoint();
builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();
host.Run();

EndpointConfiguration ConfigureEndpoint()
{
    var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Player.Tobias");
    endpointConfiguration.ConfigurePersistence(builder.Configuration["ConnectionStrings:Persistence"]);
    endpointConfiguration.ConfigureTransport(builder.Configuration["ConnectionStrings:Transport"])
                         .ConfigureRoutingOfTransport();
    endpointConfiguration.ConfigureConventions();

    endpointConfiguration.UseSerialization<SystemJsonSerializer>();
    endpointConfiguration.EnableInstallers();

    return endpointConfiguration;
}
