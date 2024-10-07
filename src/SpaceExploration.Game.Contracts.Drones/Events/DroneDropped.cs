namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneDropped(Guid PlanetId, Guid DroneId, string DroneType, string DroneName, int OverallDroneCount);
