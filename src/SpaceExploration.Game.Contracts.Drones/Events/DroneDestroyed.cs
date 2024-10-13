namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneDestroyed(Guid PlanetId, Guid DroneId, Guid DestroyedByDroneId);
