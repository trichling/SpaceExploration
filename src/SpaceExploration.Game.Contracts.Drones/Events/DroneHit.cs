namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneHit(Guid PlanetId, Guid TargetDroneId, Guid ShootingDroneId, int RemainingHealth);
