namespace SpaceExploration.Game.Contracts.Commands;

public record Shot(Guid PlanetId, Guid DroneId, Guid TargetId);