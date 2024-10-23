namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneScoreUpdated(Guid DroneSignature, int Score);