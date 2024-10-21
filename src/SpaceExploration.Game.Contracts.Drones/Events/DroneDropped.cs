namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneDropped(Guid PlanetId, Guid DroneSignature, string DroneType, string DroneName, int OverallDroneCount);
