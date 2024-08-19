using SpaceExploration.Game;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

var endpointConfiguration = new EndpointConfiguration("SpaceExploration.Game");
endpointConfiguration.UsePersistence<LearningPersistence>();

var transport = endpointConfiguration.UseTransport<LearningTransport>();
transport.StorageDirectory("..\\transport");

endpointConfiguration.UseSerialization<SystemJsonSerializer>();

builder.UseNServiceBus(endpointConfiguration);

var host = builder.Build();
host.Run();
