namespace SpaceExploration.Game.Contracts.Commands;

public record Turn(Guid PlanetId, Guid DroneId, int Angle);