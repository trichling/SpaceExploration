namespace SpaceExploration.Game.Contracts.Drones.Events;

public record DroneHit(Guid PlanetId, Guid TargetDroneSignature, Guid ShootingDroneSignature, int RemainingHealth);
