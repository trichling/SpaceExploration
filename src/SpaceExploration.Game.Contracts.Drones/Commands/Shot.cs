namespace SpaceExploration.Game.Contracts.Drones.Commands;

public record Shot(Guid PlanetId, Guid DroneId, Guid TargetId);