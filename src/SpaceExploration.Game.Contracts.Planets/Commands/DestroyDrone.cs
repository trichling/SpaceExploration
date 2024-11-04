namespace SpaceExploration.Game.Contracts.Planets.Commands;

public record DestroyDrone(Guid PlanetId, Guid DroneId);