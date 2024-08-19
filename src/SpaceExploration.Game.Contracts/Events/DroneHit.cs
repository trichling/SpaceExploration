namespace SpaceExploration.Game.Contracts.Events;

public record DroneHit(Guid PlanetId, Guid DroneId, int RemainingHealth);
