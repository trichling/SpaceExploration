namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneScoreUpdated(Guid PlanetId, Guid DroneSignature, int Score);