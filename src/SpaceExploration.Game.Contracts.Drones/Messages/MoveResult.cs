namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record MoveResult(Guid PlanetId, Guid DroneId, double DeltaX, double DeltaY);