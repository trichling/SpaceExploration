using Microsoft.Data.SqlClient;
using SpaceExploration.Game;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddHostedService<Worker>();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Game");

// var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
// persistence.SagaStorageDirectory("..\\sagas");
//var persistence = endpointConfiguration.UsePersistence<NonDurablePersistence>();

// var transport = endpointConfiguration.UseTransport<LearningTransport>();
// transport.StorageDirectory("..\\transport");

var persistence = endpointConfiguration.UsePersistence<SqlPersistence>();
persistence.SqlDialect<SqlDialect.MsSqlServer>();
persistence.ConnectionBuilder(
    connectionBuilder: () =>
    {
        return new SqlConnection(builder.Configuration["ConnectionStrings:Persistence"]);
    });

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.ConnectionString(builder.Configuration["ConnectionStrings:AzureServiceBus"]); 
transport.SubscriptionRuleNamingConvention(type => type.Name);

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

endpointConfiguration.EnableInstallers();

builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();
host.Run();

