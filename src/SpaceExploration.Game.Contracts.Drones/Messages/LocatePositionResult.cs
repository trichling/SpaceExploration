namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record LocatePositionResult(Guid DroneId, Guid DroneSignature, double Heading, double PositionX, double PositionY);