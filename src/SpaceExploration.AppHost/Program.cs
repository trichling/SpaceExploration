using Projects;

var builder = DistributedApplication.CreateBuilder(args);

var game = builder.AddProject<SpaceExploration_Game>("spaceexplorationgame");

builder.Build().Run();
