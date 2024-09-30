using Projects;

var builder = DistributedApplication.CreateBuilder(args);



var game = builder.AddProject<SpaceExploration_Game>("game");
var testclient = builder.AddProject("player", "..\\SpaceExploration.TestClient\\SpaceExploration.TestClient.csproj");    
var ui = builder.AddProject("ui", "..\\SpaceExploration.Game.Ui\\SpaceExploration.Game.Ui.csproj");    

builder.Build().Run();
