namespace SpaceExploration.Game.Contracts.Planets.Commands;
public record DropDrone(Guid DroneId, Guid PlanetId, string DroneType = "", string DroneName = "");