namespace SpaceExploration.Game.Contracts.Drones.Commands;

public record Turn(Guid PlanetId, Guid DroneId, int Angle);