using SpaceExploration.Game;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Game");
var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
persistence.SagaStorageDirectory("..\\sagas");

var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("..\\transport");

var conventions = endpointConfiguration.Conventions();
conventions.DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"));
conventions.DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
conventions.DefiningMessagesAs(t => t.Namespace != null && t.Namespace.EndsWith("Messages"));

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();
host.Run();
