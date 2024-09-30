using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var transportConnectionString = builder.AddParameter("AzureServiceBusConnectionString");
var persistenceConnectionString = builder.AddParameter("PersistenceConnectionString");

var game = builder.AddProject<SpaceExploration_Game>("game")
    .WithEnvironment("ConnectionStrings__AzureServiceBus", transportConnectionString)
    .WithEnvironment("ConnectionStrings__Persistence", persistenceConnectionString);

var testclient = builder.AddProject<SpaceExploration_TestClient>("player")
    .WithEnvironment("ConnectionStrings__AzureServiceBus", transportConnectionString)
    .WithEnvironment("ConnectionStrings__Persistence", persistenceConnectionString);

var ui = builder.AddProject<SpaceExploration_Game_Ui>("ui")
    .WithEnvironment("ConnectionStrings__AzureServiceBus", transportConnectionString)
    .WithEnvironment("ConnectionStrings__Persistence", persistenceConnectionString);

builder.Build().Run();
