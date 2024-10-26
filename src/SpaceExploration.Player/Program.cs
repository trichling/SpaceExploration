using SpaceExploration.Player;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();
builder.Services.AddHostedService<Player>();

var endpointConfiguration = ConfgureEndpoint();
builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();
await host.RunAsync();


EndpointConfiguration ConfgureEndpoint()
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

