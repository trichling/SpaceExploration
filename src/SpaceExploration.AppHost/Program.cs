using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var game = builder.AddProject<SpaceExploration_Game>("spaceexplorationgame");
var testclient = builder.AddProject("spaceexplorationtestclient", "..\\SpaceExploration.TestClient\\SpaceExploration.TestClient.csproj");    

builder.Build().Run();
