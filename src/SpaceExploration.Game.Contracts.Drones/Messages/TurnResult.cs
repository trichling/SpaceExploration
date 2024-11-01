namespace SpaceExploration.Game.Contracts.Drones.Messages;

public record TurnResult(Guid DroneId, Guid DroneSignature, int Heading);