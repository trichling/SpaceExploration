namespace SpaceExploration.Game.Contracts.Planets.Commands;

public record DestroyDronesOfType(Guid PlanetId, string DroneType);