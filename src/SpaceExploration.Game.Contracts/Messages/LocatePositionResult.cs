namespace SpaceExploration.Game.Contracts.Messages;

public record LocatePositionResult(Guid DroneId, double heading, double PositionX, double PositionY);