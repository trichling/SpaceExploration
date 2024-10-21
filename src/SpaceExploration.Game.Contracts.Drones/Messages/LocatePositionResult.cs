namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record LocatePositionResult(Guid DroneId, double Heading, double PositionX, double PositionY);