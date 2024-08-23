namespace SpaceExploration.Game.Contracts.Events;

public record DroneHit(Guid PlanetId, Guid TargetDroneId, Guid ShootingDroneId, int RemainingHealth);
