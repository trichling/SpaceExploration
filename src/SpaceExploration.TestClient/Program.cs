using System.Reflection;
using SpaceExploration.Game.Contracts.Commands;
using SpaceExploration.TestClient;

var builder = Host.CreateApplicationBuilder(args);

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.TestClient");
var persistence = endpointConfiguration.UsePersistence<LearningPersistence>();
persistence.SagaStorageDirectory("..\\sagas");

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

builder.Services.AddHostedService<Player1>();

var host = builder.Build();
host.Run();
