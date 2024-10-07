namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record LocatePositionResult(Guid DroneId, double heading, double PositionX, double PositionY);