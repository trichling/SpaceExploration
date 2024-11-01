using Microsoft.Data.SqlClient;

using SpaceExploration.Game;
using SpaceExploration.Game.Contracts.Planets.Commands;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddHostedService<Worker>();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Game");

var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.SqlDialect<SqlDialect.MsSqlServer>();
persistence.ConnectionBuilder(
    connectionBuilder: () =>
    {
        return new SqlConnection(builder.Configuration["ConnectionStrings:Persistence"]);
    });

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.ConnectionString(builder.Configuration["ConnectionStrings:Transport"]);
transport.SubscriptionRuleNamingConvention(type => type.Name);

var routing = transport.Routing();

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();
host.Run();

