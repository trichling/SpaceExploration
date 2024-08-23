using System.Reflection;
using Microsoft.Data.SqlClient;
using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.TestClient;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.TestClient");
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

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();
endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);

builder.Services.AddHostedService<Player1>();

var host = builder.Build();
host.Run();
