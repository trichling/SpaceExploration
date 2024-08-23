namespace SpaceExploration.Game.Contracts.Messages;

public record LocatePositionResult(Guid DroneId, double PositionX, double PositionY);