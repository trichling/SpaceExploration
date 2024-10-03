namespace SpaceExploration.Game.Contracts.Events;

public record DroneDropped(Guid PlanetId, Guid DroneId, string DroneType, string DroneName, int OverallDroneCount);
